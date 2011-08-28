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

        BulletParams m_params;

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public BulletEntity()
        {
            m_params = kDefaultParams;
        }


        public override void Update(ref FrameTime frameTime)
        {
            base.Update(ref frameTime);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(GetPosition(), GetPosition() - GetVelocity() * 0.02f, new Color(1.0f, 0.80f, 0.0f, 0.4f), 3.5f);
        }


        public void AddToBin(Bin bin)
        {
            AddToBin(bin, GetPosition(), GetPosition(), BinLayers.kBullet);
        }


        public void RemoveFromBin()
        {
            RemoveFromBin(BinLayers.kBullet);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = GetBin();

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromUnsortedCorners(GetLastFramePosition(), GetPosition(), ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, BinLayers.kBullet);

            SetBinRegion(curBinRegion);
        }


        public void OnCollisionEvent(ref AiEntity entity)
        {
            RemoveFromBin(BinLayers.kBullet);
            DestroyItem();
        }


        public void OnCollisionEvent(ref BoundaryEntity entity)
        {
            RemoveFromBin(BinLayers.kBullet);
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

        public BulletParams GetParams()
        {
            return m_params;
        }

        public class BulletParams
        {
            public BulletParams(float damage)
            {
                m_damage = damage;
            }

            public float m_damage;
        }

        public readonly BulletParams kDefaultParams = new BulletParams(10.0f);
    }
}
