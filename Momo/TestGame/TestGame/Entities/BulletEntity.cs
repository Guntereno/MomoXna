using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Primitive2D;
using Momo.Core.Collision2D;
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
        private BulletParams m_params;
        private CollidableGroupInfo m_collidableGroupInfo = new CollidableGroupInfo();


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public BulletParams Params
        {
            get { return m_params; }
            set { m_params = value; }
        }

        public CollidableGroupInfo CollidableGroupInfo
        {
            get { return m_collidableGroupInfo; }
            set { m_collidableGroupInfo = value; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public BulletEntity()
        {
            m_params = kDefaultParams;

            CollidableGroupInfo.GroupMembership = new Flags((int)EntityGroups.AllBullets);
            CollidableGroupInfo.CollidesWithGroups = new Flags((int)(EntityGroups.Players | EntityGroups.Enemies | EntityGroups.AllBoundaries));
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(GetPosition(), GetPosition() - Velocity * 0.02f, m_params.m_debugColor, 3.5f);
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
            bin.GetBinRegionFromUnsortedCorners(LastFramePosition, GetPosition(), ref curBinRegion);

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

        public class BulletParams
        {
            public BulletParams(float damage, Color debugColor)
            {
                m_damage = damage;
                m_debugColor = debugColor;
            }

            public float m_damage;
            public Color m_debugColor;
        }

        public static readonly BulletParams kDefaultParams = new BulletParams(10.0f, new Color(1.0f, 0.80f, 0.0f, 0.4f));
    }
}
