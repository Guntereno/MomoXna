using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Entities;
using Momo.Core.ObjectPools;
using Momo.Core;
using Momo.Debug;
using Microsoft.Xna.Framework;
using Momo.Core.Spatial;

namespace TestGame.Systems
{
    public class ProjectileManager
    {
        const int kMaxBullets = 1000;

        public ProjectileManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;
        }

        public void Load()
        {
            for (int i = 0; i < kMaxBullets; ++i)
            {
                m_bullets.AddItem(new BulletEntity(), false);
            }
        }

        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < m_bullets.ActiveItemListCount; ++i)
            {
                m_bullets[i].Update(ref frameTime);
                m_bullets[i].UpdateBinEntry();
            }
        }

        public void EndFrame()
        {
            // Destroying dead entities/objects
            m_bullets.CoalesceActiveList(false);
        }

        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_bullets.ActiveItemListCount; ++i)
            {
                m_bullets[i].DebugRender(debugRenderer);
            }
        }

        public void AddBullet(Vector2 startPos, Vector2 velocity, BulletEntity.Params param)
        {
            BulletEntity bullet = m_bullets.CreateItem();
            bullet.SetPosition(startPos);
            bullet.SetVelocity(velocity);
            bullet.SetParams(param);

            bullet.AddToBin(m_bin);
        }

        public Pool<BulletEntity> GetBullets() { return m_bullets; }

        private GameWorld m_world = null;
        private Pool<BulletEntity> m_bullets = new Pool<BulletEntity>(kMaxBullets);

        Bin m_bin = null;

    }
}
