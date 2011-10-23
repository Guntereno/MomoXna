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
        public enum Flags
        {
            None = 0x0,

            HarmsPlayer = 0x1,
            HarmsEnemies = 0x2,

            HarmsEverything = HarmsPlayer | HarmsEnemies
        }

        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private bool m_destroyed = false;
        private Params m_params;
        Flags m_flags;

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public BulletEntity()
        {
            m_params = kDefaultParams;
        }

        public void SetFlags(Flags flags)
        {
            m_flags = flags;
        }

        public Flags GetFlags()
        {
            return m_flags;
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(GetPosition(), GetPosition() - GetVelocity() * 0.02f, m_params.m_debugColor, 3.5f);
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


        public void OnCollisionEvent(ref GameEntity entity)
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

        public Params GetParams()
        {
            return m_params;
        }

        public void SetParams(Params value)
        {
            m_params = value;
        }

        public class Params
        {
            public Params(float damage, Color debugColor)
            {
                m_damage = damage;
                m_debugColor = debugColor;
            }

            public float m_damage;
            public Color m_debugColor;
        }

        public readonly Params kDefaultParams = new Params(10.0f, new Color(1.0f, 0.80f, 0.0f, 0.4f));
    }
}
