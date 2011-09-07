using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MapData
{
    using VFormat = VertexPositionTexture;

    public class Patch
    {
        public Patch()
        {
        }

        public BoundingBox GetBoundingBox()
        {
            return m_boundingBox;
        }

        public void Render(BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < m_meshes.Length; ++i)
            {
                m_meshes[i].Render(effect, graphicsDevice);
            }
        }

        private void RecalculateBoundingBox()
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < m_meshes.Length; ++i)
            {
                Mesh mesh = m_meshes[i];
                BoundingBox boundingBox = mesh.GetBoundingBox();

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

                m_boundingBox = new BoundingBox(min, max);
            }
        }

        private class Mesh
        {
            public Mesh(Texture2D texture, VFormat[] vertices)
            {
                System.Diagnostics.Debug.Assert(texture != null);
                System.Diagnostics.Debug.Assert(vertices.Length > 0);

                m_texture = texture;
                m_vertices = vertices;

                CalculateBoundingBox();
            }

            public BoundingBox GetBoundingBox()
            {
                return m_boundingBox;
            }

            public void Render(BasicEffect effect, GraphicsDevice graphicsDevice)
            {
                effect.Texture = m_texture;

                for (int p = 0; p < effect.CurrentTechnique.Passes.Count; p++)
                {
                    EffectPass pass = effect.CurrentTechnique.Passes[p];
                    pass.Apply();

                    graphicsDevice.DrawUserPrimitives<VFormat>(PrimitiveType.TriangleList, m_vertices, 0, m_vertices.Length / 3);
                }
            }

            public void CalculateBoundingBox()
            {
                Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                for (int i = 0; i < m_vertices.Length; ++i)
                {
                    if (m_vertices[i].Position.X < min.X)
                        min.X = m_vertices[i].Position.X;
                    if (m_vertices[i].Position.Y < min.Y)
                        min.Y = m_vertices[i].Position.Y;
                    if (m_vertices[i].Position.Z < min.Z)
                        min.Z = m_vertices[i].Position.Z;

                    if (m_vertices[i].Position.X > max.X)
                        max.X = m_vertices[i].Position.X;
                    if (m_vertices[i].Position.Y > max.Y)
                        max.Y = m_vertices[i].Position.Y;
                    if (m_vertices[i].Position.Z > max.Z)
                        max.Z = m_vertices[i].Position.Z;

                    m_boundingBox = new BoundingBox(min, max);
                }
            }

            private Texture2D m_texture = null;
            private VFormat[] m_vertices = null;
            BoundingBox m_boundingBox;
        }

        Mesh[] m_meshes;
        BoundingBox m_boundingBox;

        internal void Read(Map map, Microsoft.Xna.Framework.Content.ContentReader input)
        {
            int numMeshes = input.ReadInt32();

            if (numMeshes > 100)
                return;
            m_meshes = new Mesh[numMeshes];
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
                m_meshes[meshId] = new Mesh(texture, vertices);
            }

            RecalculateBoundingBox();
        }
    }
}
