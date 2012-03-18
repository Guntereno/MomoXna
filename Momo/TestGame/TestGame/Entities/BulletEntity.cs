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



namespace Game.Entities
{
    public class BulletEntity : ProjectileGameEntity, IPoolItem
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Zone mZone = null;
        private bool mDestroyed = false;
        private BulletParams mParams;
        private CollidableGroupInfo mCollidableGroupInfo = new CollidableGroupInfo();


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public Zone Zone
        {
            get { return mZone; }
            set { mZone = value; }
        }

        public BulletParams Params
        {
            get { return mParams; }
            set { mParams = value; }
        }

        public CollidableGroupInfo CollidableGroupInfo
        {
            get { return mCollidableGroupInfo; }
            set { mCollidableGroupInfo = value; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public BulletEntity(Zone zone)
        {
            mZone = zone;

            mParams = kDefaultParams;

            CollidableGroupInfo.GroupMembership = new Flags((int)EntityGroups.AllBullets);
            CollidableGroupInfo.CollidesWithGroups = new Flags((int)(EntityGroups.Players | EntityGroups.Enemies | EntityGroups.AllBoundaries));
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(GetPosition(), GetPosition() - Velocity * 0.02f, mParams.m_debugColor, 3.5f);
        }


        public void AddToBin(Bin bin)
        {
            AddToBin(bin, GetPosition(), GetPosition(), BinLayers.kBullet);
        }


        public void RemoveFromBin()
        {
            RemoveFromBin( Zone.Bin, BinLayers.kBullet);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = Zone.Bin;

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromUnsortedCorners(LastFramePosition, GetPosition(), ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, BinLayers.kBullet);

            SetBinRegion(curBinRegion);
        }


        public void OnCollisionEvent(ref GameEntity entity)
        {
            DestroyItem();
        }


        public void OnCollisionEvent(ref BoundaryEntity entity)
        {
            DestroyItem();
        }


        public bool IsDestroyed()
        {
            return mDestroyed;
        }

        public void DestroyItem()
        {
            RemoveFromBin(Zone.Bin, BinLayers.kBullet);
            mDestroyed = true;
        }

        public void ResetItem()
        {
            mDestroyed = false;
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
