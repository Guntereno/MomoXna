using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Primitive2D;
using Momo.Core.Spatial;
using Momo.Core.ObjectPools;
using Momo.Debug;



namespace TestGame.Entities
{
    public class BulletEntity : ProjectileGameEntity, IPoolItem
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private bool m_destroyed = false;


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
            AddToBin(bin, GetPosition(), GetPosition(), 2);
        }


        public void RemoveFromBin()
        {
            RemoveFromBin(2);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = GetBin();

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromUnsortedCorners(GetLastFramePosition(), GetPosition(), ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, 2);

            SetBinRegion(curBinRegion);
        }


        public void OnCollisionEvent(ref AiEntity entity)
        {
            RemoveFromBin(2);
            DestroyItem();
        }


        public void OnCollisionEvent(ref BoundaryEntity entity)
        {
            RemoveFromBin(2);
            DestroyItem();
        }


        public bool IsDestroyed()
        {
            return m_destroyed;
        }

        public void DestroyItem()
        {
            m_destroyed = true;
        }

        public void ResetItem()
        {
            m_destroyed = false;
        }
    }
}
