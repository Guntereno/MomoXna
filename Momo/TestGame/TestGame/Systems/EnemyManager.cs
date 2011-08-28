using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.ObjectPools;
using TestGame.Entities;
using Momo.Core.Spatial;
using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Debug;

namespace TestGame.Systems
{
    public class EnemyManager
    {
        public EnemyManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;
        }

        private GameWorld m_world;
        private Bin m_bin;

        private Pool<AiEntity> m_enemies = new Pool<AiEntity>(2000);

        public AiEntity Create(Vector2 pos)
        {
            AiEntity ai = new AiEntity(m_world);
            ai.SetPosition(pos);
            ai.AddToBin(m_bin);
            m_enemies.AddItem(ai, true);

            ai.Init();

            return ai;
        }

        public void Update(ref FrameTime frameTime)
        {
            bool needsCoalesce = false;
            for (int i = 0; i < m_enemies.ActiveItemListCount; ++i)
            {
                m_enemies[i].Update(ref frameTime);
                m_enemies[i].UpdateBinEntry();

                if (m_enemies[i].IsDestroyed())
                {
                    m_bin.RemoveBinItem(m_enemies[i], BinLayers.kAiEntity);
                    needsCoalesce = true;
                }
            }

            if (needsCoalesce)
            {
                m_enemies.CoalesceActiveList(false);
            }
        }

        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_enemies.ActiveItemListCount; ++i)
            {
                m_enemies[i].DebugRender(debugRenderer);
            }
        }

        public Pool<AiEntity> GetEnemies() { return m_enemies; }

    }
}
