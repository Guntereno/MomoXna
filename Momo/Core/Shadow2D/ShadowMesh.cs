using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Momo.Core;
using Momo.Core.Primitive2D;
using Momo.Core.GameEntities;
using Momo.Debug;
using Momo.Maths;



namespace Momo.Core.Shadow2D
{
    public class ShadowMesh
    {
        internal struct MeshInfo
        {
            internal bool m_prenumbra;
            internal bool m_lastEdgeShadowed;
            internal Vector2 m_lastEdge;

            internal Vector2 mLightToRootNormalized;
            internal Vector2 mPhysicalLightEdgeToRoot1;
            internal Vector2 mPhysicalLightEdgeToRoot2;
        };

        private const int kMeshInfoCapacity = 10000;
        private MeshInfo[] m_meshInfoCalculations = new MeshInfo[kMeshInfoCapacity];


        private BasicEffect m_effect = null;
        private Texture2D m_penumbraTexture = null;
        private SamplerState m_samplerState;

        private int m_penumbraVertexCnt = 0;
        private int m_penumbraVertexCapacity = 0;
        private VertexPositionTexture[] m_penumbraVerts = null;

        private int m_umbraVertexCnt = 0;
        private int m_umbraVertexCapacity = 0;
        private VertexPositionTexture[] m_umbraVerts = null;



        public void Init(int prenumbraTriCapacity, int umbraTriCapacity, Texture2D penumbraTexture, GraphicsDevice graphicsDevice)
        {
            m_effect = new BasicEffect(graphicsDevice);
            m_effect.World = Matrix.Identity;
            m_effect.TextureEnabled = true;
            m_effect.LightingEnabled = false;
            m_effect.VertexColorEnabled = false;

            m_penumbraTexture = penumbraTexture;

            m_effect.Texture = m_penumbraTexture;
            m_effect.TextureEnabled = true;

            m_samplerState = new SamplerState();
            m_samplerState.AddressU = TextureAddressMode.Clamp;
            m_samplerState.AddressV = TextureAddressMode.Clamp;
            m_samplerState.AddressW = TextureAddressMode.Clamp;
            m_samplerState.MaxAnisotropy = 0;
            m_samplerState.MaxMipLevel = 0;
            m_samplerState.MipMapLevelOfDetailBias = 0;

            m_penumbraVertexCnt = 0;
            m_penumbraVertexCapacity = prenumbraTriCapacity * 3;
            m_penumbraVerts = new VertexPositionTexture[m_penumbraVertexCapacity];

            m_umbraVertexCnt = 0;
            m_umbraVertexCapacity = umbraTriCapacity * 3;
            m_umbraVerts = new VertexPositionTexture[m_umbraVertexCapacity];
        }


        public void DeInit()
        {
            m_umbraVerts = null;
            m_penumbraVerts = null;

            m_effect.Dispose();
            m_effect = null;
        }


        public void Clear()
        {
            m_penumbraVertexCnt = 0;
            m_umbraVertexCnt = 0;
        }


        public void AddOcculudingGeometry(Vector2[] points)
        {


        }

        public void AddOcculudingGeometry(Vector2 lightPos, float lightRadius, float lightRange, Vector2[] points)
        {
            Vector2 lastPoint = points[0];
            CalculatePhysicalLightEdgesToRoot(lightPos, lightRadius, lastPoint,
                                                ref m_meshInfoCalculations[0].mLightToRootNormalized,
                                                ref m_meshInfoCalculations[0].mPhysicalLightEdgeToRoot1,
                                                ref m_meshInfoCalculations[0].mPhysicalLightEdgeToRoot2);
            m_meshInfoCalculations[0].m_lastEdge = lastPoint - points[points.Length - 2];
            m_meshInfoCalculations[0].m_lastEdgeShadowed = IsEdgeShadowCaster(m_meshInfoCalculations[0].m_lastEdge, m_meshInfoCalculations[0].mPhysicalLightEdgeToRoot1, m_meshInfoCalculations[0].mPhysicalLightEdgeToRoot2);

            for (int i = 1; i < points.Length; ++i)
            {
                Vector2 point = points[i];

                CalculatePhysicalLightEdgesToRoot(  lightPos, lightRadius, point,
                                                    ref m_meshInfoCalculations[i].mLightToRootNormalized,
                                                    ref m_meshInfoCalculations[i].mPhysicalLightEdgeToRoot1,
                                                    ref m_meshInfoCalculations[i].mPhysicalLightEdgeToRoot2);

                m_meshInfoCalculations[i].m_lastEdge = point - lastPoint;
                m_meshInfoCalculations[i].m_lastEdgeShadowed = IsEdgeShadowCaster(m_meshInfoCalculations[i].m_lastEdge, m_meshInfoCalculations[i].mPhysicalLightEdgeToRoot1, m_meshInfoCalculations[i].mPhysicalLightEdgeToRoot2);

                bool penumbra = (m_meshInfoCalculations[i - 1].m_lastEdgeShadowed != m_meshInfoCalculations[i].m_lastEdgeShadowed);
                m_meshInfoCalculations[i - 1].m_prenumbra = penumbra;

                lastPoint = point;
            }



            m_meshInfoCalculations[points.Length - 1].m_prenumbra = m_meshInfoCalculations[0].m_prenumbra;


            bool validLastShadowPoint = false;
            Vector2 lastShadowPoint = Vector2.Zero;


            if (m_meshInfoCalculations[0].m_prenumbra)
            {
                Vector2 lastV0 = Vector2.Zero;
                Vector2 lastV1 = Vector2.Zero;
                Vector2 point = points[0];
                Vector2 dPos = point - lightPos;
                Vector2 dPosNormalised = Vector2.Normalize(dPos);
                Vector2 dPosPerp = Maths2D.Perpendicular(dPosNormalised);
                CalculatePrenumbraEdgePoints(   lightPos, lightRadius, lightRange, point, dPosPerp,
                                                m_meshInfoCalculations[0].mPhysicalLightEdgeToRoot1,
                                                m_meshInfoCalculations[0].mPhysicalLightEdgeToRoot2,
                                                ref lastV0, ref lastV1);

                validLastShadowPoint = true;
                lastShadowPoint = lastV1;
            }


            lastPoint = points[0];

            for (int i = 1; i < points.Length; ++i)
            {
                Vector2 point = points[i];


                Vector2 dPos1 = point - lightPos;
                Vector2 dPos1Normalised = Vector2.Normalize(dPos1);
                Vector2 v0 = lastShadowPoint;
                Vector2 v1 = lightPos + (dPos1Normalised * lightRange);
                Vector2 v2 = Vector2.Zero;

                bool validShadowPoint = false;


                if (m_meshInfoCalculations[i].m_prenumbra)
                {
                    Vector2 dPosPerp = Maths2D.Perpendicular(dPos1Normalised);
                    CalculatePrenumbraEdgePoints(lightPos, lightRadius, lightRange, point, dPosPerp,
                                                    m_meshInfoCalculations[i].mPhysicalLightEdgeToRoot1,
                                                    m_meshInfoCalculations[i].mPhysicalLightEdgeToRoot2,
                                                    ref v1, ref v2);

                    AddShadowPrenumbra(point, v1, v2);

                    validShadowPoint = true;
                    lastShadowPoint = v2;
                }


                if (m_meshInfoCalculations[i].m_lastEdgeShadowed)
                {
                    if (!validLastShadowPoint)
                    {
                        Vector2 lastDPos = lastPoint - lightPos;
                        Vector2 lastDPos0Normalised = Vector2.Normalize(lastDPos);
                        v0 = lightPos + (lastDPos0Normalised * lightRange);
                    }
         
                    AddShadowUmbra(v0, v1, lastPoint, point);
                }


                validLastShadowPoint = validShadowPoint;
                lastPoint = point;
            }
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_penumbraVertexCnt; i += 3)
            {
                debugRenderer.DrawFilledTriangle(   m_penumbraVerts[i + 0].Position,
                                                    m_penumbraVerts[i + 1].Position,
                                                    m_penumbraVerts[i + 2].Position,
                                                    new Color(0.0f, 0.0f, 0.0f, 0.3f));
            }

            for( int i = 0; i < m_umbraVertexCnt; i += 3)
            {
                debugRenderer.DrawFilledTriangle(   m_umbraVerts[i + 0].Position,
                                                    m_umbraVerts[i + 1].Position,
                                                    m_umbraVerts[i + 2].Position,
                                                    new Color(0.0f, 0.0f, 0.0f, 0.5f));
            }
        }


        public void Render(Matrix viewMatrix, Matrix projMatrix, GraphicsDevice graphicsDevice)
        {
            m_effect.View = viewMatrix;
            m_effect.Projection = projMatrix;

            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = m_samplerState;


            for (int p = 0; p < m_effect.CurrentTechnique.Passes.Count; p++)
            {
                EffectPass pass = m_effect.CurrentTechnique.Passes[p];
                pass.Apply();

                if (m_penumbraVertexCnt > 0)
                {
                    graphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, m_penumbraVerts, 0, m_penumbraVertexCnt / 3);
                }

                if (m_umbraVertexCnt > 0)
                {
                    graphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, m_umbraVerts, 0, m_umbraVertexCnt / 3);
                }
            }


            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
        }


        // v0-4: top left, top right, bottom left, bottom right
        private void AddShadowUmbra(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float kDepth = 0.5f;
            Vector2 kUvs = new Vector2( 0.7f, 0.7f );

            int startVertIdx = m_umbraVertexCnt;

            m_umbraVerts[startVertIdx + 0] = new VertexPositionTexture(new Vector3(v0, kDepth), kUvs);
            m_umbraVerts[startVertIdx + 1] = new VertexPositionTexture(new Vector3(v1, kDepth), kUvs);
            m_umbraVerts[startVertIdx + 2] = new VertexPositionTexture(new Vector3(v2, kDepth), kUvs);
            m_umbraVerts[startVertIdx + 3] = m_umbraVerts[startVertIdx + 2];
            m_umbraVerts[startVertIdx + 4] = m_umbraVerts[startVertIdx + 1];
            m_umbraVerts[startVertIdx + 5] = new VertexPositionTexture(new Vector3(v3, kDepth), kUvs);

            m_umbraVertexCnt += 6;
        }


        // v0: origin
        // v1: umbra edge
        // v2: far edge
        private void AddShadowPrenumbra(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            float kDepth = 0.5f;

            int startVertIdx = m_penumbraVertexCnt;

            m_penumbraVerts[startVertIdx + 0] = new VertexPositionTexture(new Vector3(v0, kDepth), new Vector2(0.0f, 1.0f));
            m_penumbraVerts[startVertIdx + 1] = new VertexPositionTexture(new Vector3(v1, kDepth), new Vector2(0.0f, 0.0f));
            m_penumbraVerts[startVertIdx + 2] = new VertexPositionTexture(new Vector3(v2, kDepth), new Vector2(1.0f, 0.0f));

            m_penumbraVertexCnt += 3;
        }


        private float CalculatePrenumbraAngle( float distanceToRoot, float sourceRadius)
        {
            return (float)Math.Atan(sourceRadius / distanceToRoot);
        }


        private void CalculatePrenumbraEdgePoints(  Vector2 lightPos, float lightPhysicalRadius, float lightRange, Vector2 rootPos, Vector2 perpToRoot,
                                                    Vector2 lightEdgeToRoot1, Vector2 lightEdgeToRoot2, ref Vector2 prenumbraPoint, ref Vector2 umbraPoint )
        {
            float distanceToRoot = lightEdgeToRoot1.Length();
            lightEdgeToRoot1 = lightEdgeToRoot1 / distanceToRoot;
            lightEdgeToRoot2 = lightEdgeToRoot2 / distanceToRoot;

            prenumbraPoint = lightPos + (lightEdgeToRoot1 * lightRange);
            umbraPoint = lightPos + (lightEdgeToRoot2 * lightRange);
        }


        private void CalculatePhysicalLightEdgesToRoot( Vector2 lightPos, float lightPhysicalRadius, Vector2 rootPos,
                                                        ref Vector2 lightToRootNormalized, ref Vector2 lightEdgeToRoot1, ref Vector2 lightEdgeToRoot2)
        {
            Vector2 lightToRoot = rootPos - lightPos;
            Vector2 lightToRootNorm = Vector2.Normalize(lightToRoot);

            Vector2 sourcePerpRadius = Maths2D.Perpendicular(lightToRootNorm) * lightPhysicalRadius;
            Vector2 sourceEdgePoint1 = lightPos - sourcePerpRadius;
            Vector2 sourceEdgePoint2 = lightPos + sourcePerpRadius;

            lightToRootNormalized = lightToRootNorm;
            lightEdgeToRoot1 = (rootPos - sourceEdgePoint1);
            lightEdgeToRoot2 = (rootPos - sourceEdgePoint2);
        }


        public bool IsEdgeShadowCaster(Vector2 edge, Vector2 lightEdgeToRoot1, Vector2 lightEdgeToRoot2)
        {
            Vector2 perpEdge = Maths2D.Perpendicular(edge);

            float dotPerp1 = Vector2.Dot(perpEdge, lightEdgeToRoot1);
            float dotPerp2 = Vector2.Dot(perpEdge, lightEdgeToRoot2);

            return ((dotPerp1 > 0.0f) || (dotPerp2 > 0.0f));
        }

    }
}
