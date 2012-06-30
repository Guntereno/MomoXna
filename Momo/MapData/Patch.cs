using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Momo.Core;
using Momo.Core.Models;

namespace MapData
{
    using VFormat = VertexPositionNormalTexture;

    public class Patch
    {
        public Mesh[] Meshes { get; private set; }
        public ModelInst[] Models { get; private set; }
        public BoundingBox BoundingBox { get; private set; }

        public Patch()
        {
        }


        public void Draw(Matrix view, Matrix projection, BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            for (int i=0; i < Meshes.Length; ++i)
            {
                Meshes[i].Draw(effect, graphicsDevice);
            }

            for (int i = 0; i < Models.Length; ++i)
            {
                Models[i].Draw(view, projection);
            }
        }

        private void RecalculateBoundingBox()
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < Meshes.Length; ++i)
            {
                Mesh mesh = Meshes[i];
                BoundingBox boundingBox = mesh.BoundingBox;

                if (boundingBox.Min.X < min.X)
                    min.X = boundingBox.Min.X;
                if (boundingBox.Min.Y < min.Y)
                    min.Y = boundingBox.Min.Y;
                if (boundingBox.Min.Z < min.Z)
                    min.Z = boundingBox.Min.Z;

                if (boundingBox.Max.X > max.X)
                    max.X = boundingBox.Max.X;
                if (boundingBox.Max.Y > max.Y)
                    max.Y = boundingBox.Max.Y;
                if (boundingBox.Max.Z > max.Z)
                    max.Z = boundingBox.Max.Z;
            }

            BoundingBox = new BoundingBox(min, max);
        }

        public class Mesh
        {
            public Texture2D Texture { get; private set; }
            public VFormat[] Vertices { get; private set; }
            public BoundingBox BoundingBox { get; private set; }

            public Mesh(Texture2D texture, VFormat[] vertices)
            {
                System.Diagnostics.Debug.Assert(texture != null);
                System.Diagnostics.Debug.Assert(vertices.Length > 0);

                Texture = texture;
                Vertices = vertices;

                CalculateBoundingBox();
            }

            public void Draw(BasicEffect effect, GraphicsDevice graphicsDevice)
            {
                effect.Texture = Texture;

                effect.EnableDefaultLighting();

                for (int p = 0; p < effect.CurrentTechnique.Passes.Count; p++)
                {
                    EffectPass pass = effect.CurrentTechnique.Passes[p];
                    pass.Apply();

                    graphicsDevice.DrawUserPrimitives<VFormat>(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length / 3);
                }
            }

            public void CalculateBoundingBox()
            {
                Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                for (int i = 0; i < Vertices.Length; ++i)
                {
                    if (Vertices[i].Position.X < min.X)
                        min.X = Vertices[i].Position.X;
                    if (Vertices[i].Position.Y < min.Y)
                        min.Y = Vertices[i].Position.Y;
                    if (Vertices[i].Position.Z < min.Z)
                        min.Z = Vertices[i].Position.Z;

                    if (Vertices[i].Position.X > max.X)
                        max.X = Vertices[i].Position.X;
                    if (Vertices[i].Position.Y > max.Y)
                        max.Y = Vertices[i].Position.Y;
                    if (Vertices[i].Position.Z > max.Z)
                        max.Z = Vertices[i].Position.Z;
                }

                BoundingBox = new BoundingBox(min, max);
            }


        }

        internal void Read(Map map, Microsoft.Xna.Framework.Content.ContentReader input)
        {
            int numMeshes = input.ReadInt32();

            if (numMeshes > 100)
                return;
            Meshes = new Mesh[numMeshes];
            for (int meshId = 0; meshId < numMeshes; ++meshId)
            {
                int tilesetIdx = input.ReadInt32();
                int vertexCount = input.ReadInt32();
                VFormat[] vertices = new VFormat[vertexCount];
                for (int vertId = 0; vertId < vertexCount; ++vertId)
                {
                    vertices[vertId] = input.ReadObject<VFormat>();
                }

                Texture2D texture = map.Tilesets[tilesetIdx].DiffuseMap;
                Meshes[meshId] = new Mesh(texture, vertices);
            }

            int numModels = input.ReadInt32();
            Models = new ModelInst[numModels];
            for (int modelId = 0; modelId < numModels; ++modelId)
            {
                string modelName = input.ReadString();
                Matrix worldMatrix = input.ReadObject<Matrix>();

                Model model = ResourceManager.Instance.Get<Model>(modelName);

                Models[modelId] = new ModelInst(model, worldMatrix);
            }

            RecalculateBoundingBox();
        }
    }
}
