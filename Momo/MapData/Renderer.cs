using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapData
{
    using VFormat = VertexPositionTexture;

    public class Renderer
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        bool m_inited = false;
        MapData.Map m_map = null;

        BasicEffect m_effect = null;

        SamplerState m_samplerState;

        BoundingFrustum m_viewFrustum = new BoundingFrustum(Matrix.Identity);


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void Init(MapData.Map map, GraphicsDevice graphicsDevice, int patchSize)
        {
            m_map = map;
            m_inited = true;

            m_effect = new BasicEffect(graphicsDevice);
            m_effect.TextureEnabled = true;
            m_effect.LightingEnabled = false;
            m_effect.VertexColorEnabled = false;

            m_samplerState = new SamplerState();
            m_samplerState.AddressU = TextureAddressMode.Clamp;
            m_samplerState.AddressV = TextureAddressMode.Clamp;
            m_samplerState.AddressW = TextureAddressMode.Clamp;
            m_samplerState.Filter = TextureFilter.Point;
            m_samplerState.MaxAnisotropy = 0;
            m_samplerState.MaxMipLevel = 0;
            m_samplerState.MipMapLevelOfDetailBias = 0;
        }

        public void Render(Matrix viewMatrix, Matrix projMatrix, GraphicsDevice graphicsDevice)
        {
            if (!m_inited)
                return;

            Matrix modelMatrix = Matrix.CreateScale(1.0f, 1.0f, 1.0f);

            m_effect.World = modelMatrix;
            m_effect.View = viewMatrix;
            m_effect.Projection = projMatrix;

            // Create a view frustrum for culling
            m_viewFrustum.Matrix = modelMatrix * viewMatrix * projMatrix;

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = m_samplerState;

            for (int layerIdx = 0; layerIdx < m_map.LayerCount; ++layerIdx)
            {
                for (int patchIdx = 0; patchIdx < m_map.Patches[layerIdx].Length; ++patchIdx)
                {
                    MapData.Patch patch = m_map.Patches[layerIdx][patchIdx];
                    if (m_viewFrustum.Intersects(patch.BoundingBox))
                    {
                        patch.Draw(viewMatrix, projMatrix, m_effect, graphicsDevice);
                    }
                }
            }

            for (int modelIdx = 0; modelIdx < m_map.ModelInstances.Length; ++modelIdx)
            {
                MapData.ModelInst modelInst = m_map.ModelInstances[modelIdx];
                modelInst.Draw(viewMatrix, projMatrix);
            }

            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

    }
}
