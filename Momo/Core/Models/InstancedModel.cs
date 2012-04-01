using System;

using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Momo.Core.Models
{
    public class InstancedModel
    {
        int mInstanceCapacity = 1000;
        int mInstanceCount = 0;

        InstanceData[] mInstanceDataList = null;
        Matrix[] mMeshLocalMatrix = null;
        DynamicVertexBuffer mInstanceVertexBuffer = null;

        Model mInstancedModel = null;
        Effect mInstanceEffect = null;



        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration kInstanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );



        public void Init(int instanceCapacity, Model model, Effect effect, GraphicsDevice graphicsDevice)
        {
            mInstanceCapacity = instanceCapacity;
            mInstanceCount = 0;

            mInstanceDataList = new InstanceData[instanceCapacity];

            mMeshLocalMatrix = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(mMeshLocalMatrix);
            
            mInstancedModel = model;
            mInstanceEffect = effect;
            mInstanceVertexBuffer = new DynamicVertexBuffer(graphicsDevice, kInstanceVertexDeclaration,
                                                            instanceCapacity, BufferUsage.WriteOnly);
        }


        public void DeInit()
        {
            mInstanceVertexBuffer.Dispose();
            mInstanceVertexBuffer = null;
            mInstanceEffect = null;
            mInstancedModel = null;
            mInstanceDataList = null;
            mInstanceCount = 0;
        }


        public void RenderInstance(Matrix worldMatrix)
        {
            System.Diagnostics.Debug.Assert(mInstanceCount < mInstanceCapacity);

            mInstanceDataList[mInstanceCount].mWorldMatrix = worldMatrix;
            ++mInstanceCount;
        }


        public void Render(Matrix viewProjMatrix, GraphicsDevice graphicsDevice)
        {
            if (mInstanceCount > 0)
            {
                // Transfer the latest instance transform matrices into the instanceVertexBuffer.
                mInstanceVertexBuffer.SetData(mInstanceDataList, 0, mInstanceCount, SetDataOptions.Discard);


                //ModelMesh mesh = mInstancedModel.Meshes[3];
                foreach (ModelMesh mesh in mInstancedModel.Meshes)
                {
                    //ModelMeshPart meshPart = mesh.MeshParts[1];
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                        graphicsDevice.SetVertexBuffers(
                            new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                            new VertexBufferBinding(mInstanceVertexBuffer, 0, 1)
                        );

                        graphicsDevice.Indices = meshPart.IndexBuffer;

                        // Set up the instance rendering effect.
                        Effect effect = mInstanceEffect;

                        effect.CurrentTechnique = effect.Techniques[0];

                        //effect.Parameters["view"].SetValue(viewMatrix);
                        //effect.Parameters["projection"].SetValue(projMatrix);

                        effect.Parameters["world"].SetValue(mMeshLocalMatrix[mesh.ParentBone.Index]);
                        effect.Parameters["viewProjection"].SetValue(viewProjMatrix);

                        effect.CurrentTechnique.Passes[0].Apply();

                        graphicsDevice.DrawInstancedPrimitives( PrimitiveType.TriangleList, 0, 0,
                                                                meshPart.NumVertices, meshPart.StartIndex,
                                                                meshPart.PrimitiveCount, mInstanceCount);
                    }
                }


                mInstanceCount = 0;
            }
        }
    }
}
