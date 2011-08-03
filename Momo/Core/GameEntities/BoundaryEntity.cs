using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.Spatial;
using Momo.Core.Collision2D;
using Momo.Core.Primitive2D;

using Momo.Debug;



namespace Momo.Core.GameEntities
{
    public class BoundaryEntity : GameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private LineStripPrimitive2D m_collisionPrimitive = null;
        private BinRegionUniform [] m_collisionLineBinRegions = null;



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public LineStripPrimitive2D CollisionPrimitive
        {
            get { return m_collisionPrimitive; }
        }

        public BinRegionUniform [] CollisionLineBinRegions
        {
            get { return m_collisionLineBinRegions; }
        }


        public override Vector2 GetVelocity()
        {
            return Vector2.Zero;
        }


        public BoundaryEntity(LineStripPrimitive2D collisionPrimitive)
        {
            m_collisionPrimitive = collisionPrimitive;

            m_collisionLineBinRegions = new BinRegionUniform[m_collisionPrimitive.LineCount];
        }


        public void RecalculateBinRegion(Bin bin)
        {
            Vector2 minCorner;
            Vector2 maxCorner;
            BinRegionUniform binRegion = new BinRegionUniform();

            m_collisionPrimitive.CalculateBoundingArea(out minCorner, out maxCorner);

            bin.GetBinRegionCorners(minCorner, maxCorner, ref binRegion);

            SetBinRegion(binRegion);

            for (int i = 0; i < m_collisionPrimitive.LineCount; ++i)
            {
                Vector2 min;
                Vector2 max;

                m_collisionPrimitive.LineList[i].CalculateMinMax(out min, out max);

                bin.GetBinRegionCorners(min, max, ref m_collisionLineBinRegions[i]);
            }
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            System.Diagnostics.Debug.Assert(m_collisionPrimitive != null);

            Vector2 lastPoint = m_collisionPrimitive.LineList[0].m_point;

            for (int i = 1; i < m_collisionPrimitive.PointCount; ++i)
            {
                Vector2 point = m_collisionPrimitive.LineList[i].m_point;

                debugRenderer.DrawFilledLine(point, lastPoint, new Color(0.0f, 0.0f, 0.0f, 0.5f), 5.0f);

                lastPoint = point;
            }

            
        }
    }
}
