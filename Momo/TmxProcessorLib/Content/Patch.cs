using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

using VFormat = Microsoft.Xna.Framework.Graphics.VertexPositionTexture;

namespace TmxProcessorLib.Content
{
    internal class Mesh
    {
        private int m_tilesetIdx;
        private VFormat[] m_verts;

        public Mesh(int tilesetIdx, VFormat[] verts)
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

        public Patch()
        {
            Meshes = new List<Mesh>();
        }

        internal void Write(ContentWriter output)
        {
            output.Write(Meshes.Count);
            foreach (Mesh mesh in Meshes)
            {
                mesh.Write(output);
            }
        }
    }


}
