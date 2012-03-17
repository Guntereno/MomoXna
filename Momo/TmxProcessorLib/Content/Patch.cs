using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

using VFormat = Microsoft.Xna.Framework.Graphics.VertexPositionTexture;

namespace TmxProcessorLib.Content
{
    internal class ModelInst
    {
        private string m_modelName;
        private Matrix m_worldMatrix;

        internal ModelInst(string modelName, Matrix worldMatrix)
        {
            m_modelName = modelName;
            m_worldMatrix = worldMatrix;
        }

        internal void Write(ContentWriter output)
        {
            output.Write(m_modelName);
            output.WriteObject<Matrix>(m_worldMatrix);
        }
    }

    internal class Mesh
    {
        private int m_tilesetIdx;
        private VFormat[] m_verts;

        internal Mesh(int tilesetIdx, VFormat[] verts)
        {
            m_tilesetIdx = tilesetIdx;
            m_verts = verts;
        }

        internal void Write(ContentWriter output)
        {
            output.Write(m_tilesetIdx);
            output.Write(m_verts.Length);
            for (int i = 0; i < m_verts.Length; ++i)
            {
                VFormat vert = m_verts[i];
                output.WriteObject<VFormat>(vert);
            }
        }
    }

    internal class Patch
    {
        internal List<Mesh> Meshes { get; private set; }
        internal List<ModelInst> Models { get; private set; }

        public Patch()
        {
            Meshes = new List<Mesh>();
            Models = new List<ModelInst>();
        }

        internal void Write(ContentWriter output)
        {
            output.Write(Meshes.Count);
            foreach (Mesh mesh in Meshes)
            {
                mesh.Write(output);
            }

            output.Write(Models.Count);
            foreach (ModelInst modelInst in Models)
            {
                modelInst.Write(output);
            }
        }
    }


}
