using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Entities;
using Momo.Core.ObjectPools;
using Momo.Core;
using Momo.Debug;
using Microsoft.Xna.Framework;
using Momo.Core.Spatial;

namespace Game.Systems
{
    public class ProjectileManager
    {
        private const int kMaxBullets = 1000;

        private Zone mZone = null;
        private Pool<BulletEntity> mBullets = new Pool<BulletEntity>(kMaxBullets, 1, 2, false);


        public Pool<BulletEntity> Bullets       { get { return mBullets; } }


        public ProjectileManager(Zone zone)
        {
            mZone = zone;

            mBullets.RegisterPoolObjectType(typeof(BulletEntity), kMaxBullets);
        }

        public void Load()
        {
            for (int i = 0; i < kMaxBullets; ++i)
            {
                mBullets.AddItem(new BulletEntity(mZone), false);
            }
        }

        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < mBullets.ActiveItemListCount; ++i)
            {
                mBullets[i].Update(ref frameTime, 0);
                mBullets[i].UpdateBinEntry();
            }
        }

        public void PostUpdate()
        {
            mBullets.Update();
        }

        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < mBullets.ActiveItemListCount; ++i)
            {
                mBullets[i].DebugRender(debugRenderer);
            }
        }

        public void AddBullet(Vector2 startPos, Vector2 velocity, BulletEntity.BulletParams param, Flags bulletGroupMembership)
        {
            BulletEntity bullet = mBullets.CreateItem(typeof(BulletEntity));
            bullet.SetPosition(startPos);
            bullet.Velocity = velocity;
            bullet.Params = param;
            bullet.CollidableGroupInfo.GroupMembership = bulletGroupMembership;

            bullet.AddToBin(mZone.Bin);
        }
    }
}
