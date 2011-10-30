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
    public class BoundaryEntity : BaseEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private LinePrimitive2D m_collisionPrimitive;
        private BinRegionSelection m_collisionLineBinRegionsSelection;    // For axis aligned lines this is unused.


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public LinePrimitive2D CollisionPrimitive
        {
            get { return m_collisionPrimitive; }
        }

        public BinRegionSelection CollisionLineBinRegionsSelection
        {
            get { return m_collisionLineBinRegionsSelection; }
        }

        public BoundaryEntity(LinePrimitive2D collisionPrimitive)
        {
            m_collisionPrimitive = collisionPrimitive;
        }


        public void RecalculateBinRegion(Bin bin)
        {
            Vector2 minCorner;
            Vector2 maxCorner;
            m_collisionPrimitive.CalculateMinMax(out minCorner, out maxCorner);

            // Update uniform region
            BinRegionUniform binRegion = new BinRegionUniform();
            bin.GetBinRegionFromUnsortedCorners(m_collisionPrimitive.m_point, m_collisionPrimitive.m_point + m_collisionPrimitive.m_difference, ref binRegion);
            SetBinRegion(binRegion);

            // Only generate the selection on non axis aligned lines, massive waste otherwise.
            //if (binRegion.GetHeight() != 0 && binRegion.GetWidth() != 0)
            //    bin.GetBinRegionFromLine(m_collisionPrimitive.m_point, m_collisionPrimitive.m_difference, out m_collisionLineBinRegionsSelection);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            Color kBoundaryColour1 = new Color(0.0f, 1.0f, 0.0f, 0.5f);
            debugRenderer.DrawFilledLineWithCaps(m_collisionPrimitive.m_point, m_collisionPrimitive.m_point + m_collisionPrimitive.m_difference, kBoundaryColour1, 2.0f);
        }


        public void AddToBin(Bin bin, int layerIdx)
        {
            RecalculateBinRegion(bin);


            BinRegionUniform binRegion = new BinRegionUniform();
            GetBinRegion(ref binRegion);

            if(m_collisionLineBinRegionsSelection.m_binIndices == null)
                bin.UpdateBinItem(this, ref binRegion, layerIdx);
            else
                bin.UpdateBinItem(this, ref m_collisionLineBinRegionsSelection, layerIdx);
        }
    }
}
