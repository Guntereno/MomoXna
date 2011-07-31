using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Graphics
{
    using VFormat = VertexPositionColorTexture;

    public class MapRenderer
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        bool m_inited = false;
        Map.Map m_map = null;



        VFormat[] m_vertices;
        int m_currentVert = 0;

        BasicEffect m_effect = null;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void Init(Map.Map map, GraphicsDevice graphicsDevice)
        {
            m_map = map;
            m_inited = true;

            m_vertices = new VFormat[m_map.Dimensions.X * m_map.Dimensions.Y * 6];

            m_effect = new BasicEffect(graphicsDevice);
            m_effect.TextureEnabled = true;
            m_effect.LightingEnabled = false;
            m_effect.VertexColorEnabled = true;
        }

        public void Render(Matrix viewMatrix, Matrix projMatrix, GraphicsDevice graphicsDevice)
        {
            if (!m_inited)
                return;

            m_effect.World = Matrix.CreateScale(1.0f, -1.0f, 1.0f);
            m_effect.View = viewMatrix;
            m_effect.Projection = projMatrix;

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;

            for (int layerIndex = 0; layerIndex < m_map.TileLayers.Length; layerIndex++)
            {
                RenderLayer(m_map.TileLayers[layerIndex], graphicsDevice);
            }

            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
        }

        private void RenderLayer(Map.TileLayer tileLayer, GraphicsDevice graphicsDevice)
        {
            // Render tiles from each tileset separately
            for (int t = 0; t < m_map.Tilesets.Length; ++t)
            {

                Map.Tileset tileset = m_map.Tilesets[t];
                Texture2D texture = tileset.DiffuseMap;

                // Begin from the start of the vertex buffer
                m_currentVert = 0;

                for (int x = 0; x < m_map.Dimensions.X; ++x)
                {
                    for (int y = 0; y < m_map.Dimensions.Y; ++y)
                    {
                        Map.Tile tile = m_map.Tiles[tileLayer.Indices[x + (y * tileLayer.Dimensions.X)]];

                        if (tile == null)
                            continue;

                        if (tile.DiffuseMap == texture)
                        {

                            float left = (x * m_map.TileDimensions.X);
                            float right = ((x + 1) * m_map.TileDimensions.X);
                            float top = (y * m_map.TileDimensions.Y);
                            float bottom = ((y + 1) * m_map.TileDimensions.Y);

                            float texLeft = ((float)tile.Source.Left / (float)texture.Width);
                            float texRight = ((float)tile.Source.Right / (float)texture.Width);
                            float texTop = ((float)tile.Source.Top / (float)texture.Height);
                            float texBottom = ((float)tile.Source.Bottom / (float)texture.Height);

                            m_vertices[m_currentVert].Position = new Vector3(left, top, 0.0f);
                            m_vertices[m_currentVert].TextureCoordinate = new Vector2(texLeft, texTop);
                            m_vertices[m_currentVert].Color = Color.White;
                            ++m_currentVert;

                            m_vertices[m_currentVert].Position = new Vector3(right, top, 0.0f);
                            m_vertices[m_currentVert].TextureCoordinate = new Vector2(texRight, texTop);
                            m_vertices[m_currentVert].Color = Color.White;
                            ++m_currentVert;

                            m_vertices[m_currentVert].Position = new Vector3(left, bottom, 0.0f);
                            m_vertices[m_currentVert].TextureCoordinate = new Vector2(texLeft, texBottom);
                            m_vertices[m_currentVert].Color = Color.White;
                            ++m_currentVert;


                            m_vertices[m_currentVert].Position = new Vector3(left, bottom, 0.0f);
                            m_vertices[m_currentVert].TextureCoordinate = new Vector2(texLeft, texBottom);
                            m_vertices[m_currentVert].Color = Color.White;
                            ++m_currentVert;

                            m_vertices[m_currentVert].Position = new Vector3(right, top, 0.0f);
                            m_vertices[m_currentVert].TextureCoordinate = new Vector2(texRight, texTop);
                            m_vertices[m_currentVert].Color = Color.White;
                            ++m_currentVert;

                            m_vertices[m_currentVert].Position = new Vector3(right, bottom, 0.0f);
                            m_vertices[m_currentVert].TextureCoordinate = new Vector2(texRight, texBottom);
                            m_vertices[m_currentVert].Color = Color.White;
                            ++m_currentVert;
                        }

                    }
                }

                if (m_currentVert > 0)
                {
                    m_effect.Texture = texture;

                    for (int p = 0; p < m_effect.CurrentTechnique.Passes.Count; p++)
                    {
                        EffectPass pass = m_effect.CurrentTechnique.Passes[p];
                        pass.Apply();

                        graphicsDevice.DrawUserPrimitives<VFormat>(PrimitiveType.TriangleList, m_vertices, 0, m_currentVert / 3);
                    }
                }

            }
        }
    }
}
