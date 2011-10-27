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
        private int m_prenumbraVertexCnt = 0;
        private int m_prenumbraVertexCapacity = 0;
        private VertexPositionTexture[] m_prenumbraVerts = null;

        private int m_umbraVertexCnt = 0;
        private int m_umbraVertexCapacity = 0;
        private VertexPositionTexture[] m_umbraVerts = null;



        public void Init(int prenumbraTriCapacity, int umbraTriCapacity, GraphicsDevice graphicsDevice)
        {
            m_prenumbraVertexCnt = 0;
            m_prenumbraVertexCapacity = prenumbraTriCapacity * 3;
            m_prenumbraVerts = new VertexPositionTexture[m_prenumbraVertexCapacity];

            m_umbraVertexCnt = 0;
            m_umbraVertexCapacity = umbraTriCapacity * 3;
            m_umbraVerts = new VertexPositionTexture[m_umbraVertexCapacity];
        }


        public void DeInit()
        {
            m_umbraVerts = null;
            m_prenumbraVerts = null;
        }


        public void Clear()
        {
            m_prenumbraVertexCnt = 0;
            m_umbraVertexCnt = 0;
        }


        public void AddOcculudingGeometry(Vector2[] points)
        {


        }


        public void AddOcculudingGeometry(Vector2 lightPos, float lightRadius, float lightRange, List<BoundaryEntity> boundaries)
        {
            bool lastPointShadowGenerating = false;

            for (int i = 0; i < boundaries.Count; ++i)
            {
                LinePrimitive2D line = boundaries[i].CollisionPrimitive;
                Vector2 dPos0 = line.Point - lightPos;
                float dotNormal = Vector2.Dot(dPos0, line.Normal);


                if (dotNormal < 0.0f)
                {
                    Vector2 dPos1 = (line.Point + line.Difference) - lightPos;

                    Vector2 p0Normal = Vector2.Normalize(dPos0);
                    Vector2 p1Normal = Vector2.Normalize(dPos1);

                    Vector2 v0 = lightPos + (p0Normal * lightRange);
                    Vector2 v1 = lightPos + (p1Normal * lightRange);
                    Vector2 v2 = line.Point;
                    Vector2 v3 = line.Point + line.Difference;

                    // Add a prenumbra
                    if (!lastPointShadowGenerating)
                    {
                        Vector2 p0NormalPerp = Maths2D.Perpendicular(p0Normal);
                        Vector2 vPrenumbraEdgePoint;
                        Vector2 vUmbraEdgePoint;

                        CalculatePrenumbraEdgePoints(lightPos, lightRadius, lightRange, line.Point, p0NormalPerp, out vPrenumbraEdgePoint, out vUmbraEdgePoint);

                        AddShadowPrenumbra(v2, vUmbraEdgePoint, vPrenumbraEdgePoint);

                        v0 = vUmbraEdgePoint;
                    }

                    lastPointShadowGenerating = true;
                    
                    AddShadowUmbra(v0, v1, v2, v3);
                }
                else
                {
                    lastPointShadowGenerating = false;
                }
            }
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_prenumbraVertexCnt; i += 3)
            {
                debugRenderer.DrawFilledTriangle(   m_prenumbraVerts[i + 0].Position,
                                                    m_prenumbraVerts[i + 1].Position,
                                                    m_prenumbraVerts[i + 2].Position,
                                                    new Color(0.2f, 0.2f, 0.5f, 0.5f));
            }

            for( int i = 0; i < m_umbraVertexCnt; i += 3)
            {
                debugRenderer.DrawFilledTriangle(   m_umbraVerts[i + 0].Position,
                                                    m_umbraVerts[i + 1].Position,
                                                    m_umbraVerts[i + 2].Position,
                                                    new Color(0.2f, 0.2f, 1.0f, 0.5f));
            }
        }


        // v0-4: top left, top right, bottom left, bottom right
        private void AddShadowUmbra(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float kDepth = 0.5f;
            Vector2 kUvs = new Vector2( 0.0f, 0.0f );

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

            int startVertIdx = m_prenumbraVertexCnt;

            m_prenumbraVerts[startVertIdx + 0] = new VertexPositionTexture(new Vector3(v0, kDepth), new Vector2(0.0f, 1.0f));
            m_prenumbraVerts[startVertIdx + 1] = new VertexPositionTexture(new Vector3(v1, kDepth), new Vector2(1.0f, 0.0f));
            m_prenumbraVerts[startVertIdx + 2] = new VertexPositionTexture(new Vector3(v2, kDepth), new Vector2(1.0f, 1.0f));

            m_prenumbraVertexCnt += 3;
        }


        private float CalculatePrenumbraAngle( float distanceToRoot, float sourceRadius)
        {
            return (float)Math.Atan(sourceRadius / distanceToRoot);
        }


        private void CalculatePrenumbraEdgePoints(Vector2 lightPos, float lightPhysicalRadius, float lightRange, Vector2 rootPos, Vector2 perpToRoot, out Vector2 prenumbraPoint, out Vector2 umbraPoint)
        {
            Vector2 sourcePerpRadius = perpToRoot * lightPhysicalRadius;
            Vector2 sourceEdgePoint1 = lightPos - sourcePerpRadius;
            Vector2 sourceEdgePoint2 = lightPos + sourcePerpRadius;

            Vector2 edge1 = (rootPos - sourceEdgePoint1);
            float distanceToRoot = edge1.Length();
            //float prenumbraAngle = CalculatePrenumbraAngle(distanceToRoot, sourceRadius);

            Vector2 edge2 = (rootPos - sourceEdgePoint2);

            edge1 = edge1 / distanceToRoot;
            edge2 = edge2 / distanceToRoot;

            prenumbraPoint = lightPos + (edge1 * lightRange);
            umbraPoint = lightPos + (edge2 * lightRange);
        }
    }
}
