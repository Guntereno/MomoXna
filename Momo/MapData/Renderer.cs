using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Momo.Core.Models;
using Momo.Core.Nodes.Cameras;

namespace MapData
{

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
        public void Init(MapData.Map map, GraphicsDevice graphicsDevice)
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

        public void Render(CameraNode camera, GraphicsDevice graphicsDevice)
        {
            InitRender(camera, graphicsDevice);

            RenderPatches(camera, graphicsDevice);

            RenderModels(camera);

            EndRender(graphicsDevice);
        }


        protected void InitRender(CameraNode camera, GraphicsDevice graphicsDevice)
        {
            Matrix viewMatrix = camera.ViewMatrix;
            Matrix projMatrix = camera.ProjectionMatrix;

            if (!m_inited)
                return;

            Matrix modelMatrix = Matrix.CreateScale(1.0f, 1.0f, 1.0f);

            m_effect.World = modelMatrix;
            m_effect.View = viewMatrix;
            m_effect.Projection = projMatrix;
            m_effect.PreferPerPixelLighting = true;

            // Create a view frustrum for culling
            m_viewFrustum.Matrix = modelMatrix * viewMatrix * projMatrix;

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = m_samplerState;
        }

        private static void EndRender(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        private void RenderPatches(CameraNode camera, GraphicsDevice graphicsDevice)
        {

            for (int layerIdx = 0; layerIdx < m_map.LayerCount; ++layerIdx)
            {
                for (int patchIdx = 0; patchIdx < m_map.Patches[layerIdx].Length; ++patchIdx)
                {
                    MapData.Patch patch = m_map.Patches[layerIdx][patchIdx];
                    if (m_viewFrustum.Intersects(patch.BoundingBox))
                    {
                        patch.Draw(camera.ViewMatrix, camera.ProjectionMatrix, m_effect, graphicsDevice);
                    }
                }
            }
        }


        private void RenderModels(CameraNode camera)
        {
            for (int modelIdx = 0; modelIdx < m_map.ModelInstances.Length; ++modelIdx)
            {
                ModelInst modelInst = m_map.ModelInstances[modelIdx];
                modelInst.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            }
        }
    }

}
