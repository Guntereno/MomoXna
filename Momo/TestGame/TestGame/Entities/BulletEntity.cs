using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Primitive2D;
using Momo.Core.Spatial;
using Momo.Debug;



namespace TestGame.Entities
{
    public class BulletEntity : ProjectileGameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public BulletEntity()
        {

        }


        public override void Update(ref FrameTime frameTime)
        {
            base.Update(ref frameTime);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(GetPosition(), GetPosition() - GetVelocity() * 0.04f, new Color(0.0f, 1.0f, 1.0f, 0.6f), 3.0f);
        }


        public void AddToBin(Bin bin)
        {
            BinRegionUniform curBinRegion = new BinRegionUniform();

            bin.GetBinRegionFromUnsortedCorners(GetLastFramePosition(), GetPosition(), ref curBinRegion);

            SetBinRegion(curBinRegion);
        }


        public void RemoveFromBin(Bin bin)
        {
            bin.RemoveBinItem(this, 2);
        }


        public void UpdateBinEntry(Bin bin)
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromUnsortedCorners(GetLastFramePosition(), GetPosition(), ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, 2);

            SetBinRegion(curBinRegion);
        }


        public void OnCollisionEvent(ref AiEntity entity)
        {
            SetFlags(int.MaxValue);
        }


        public void OnCollisionEvent(ref BoundaryEntity entity)
        {
            SetFlags(int.MaxValue);
        }
    }
}
