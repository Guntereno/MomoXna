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
using TestGame.Entities.Enemies;

namespace TestGame.Systems
{
    public class EnemyManager
    {
        private GameWorld m_world;
        private Bin m_bin;

        private Pool<AiEntity> m_meleeEnemies = new Pool<AiEntity>(1000);
        private Pool<AiEntity> m_misileEnemies = new Pool<AiEntity>(1000);

        private int m_killCounter = 0;

        public EnemyManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;
        }


        public AiEntity Create(MapData.EnemyData.Species species, Vector2 pos)
        {
            AiEntity createdEntity = null;

            switch (species)
            {
                case MapData.EnemyData.Species.Melee:
                    {
                        MeleeEnemy ai = new MeleeEnemy(m_world);
                        m_meleeEnemies.AddItem(ai, true);
                        createdEntity = ai;
                        ai.Init();
                    }
                    break;


                case MapData.EnemyData.Species.Missile:
                    {
                        MissileEnemy ai = new MissileEnemy(m_world);
                        m_meleeEnemies.AddItem(ai, true);
                        createdEntity = ai;
                        ai.Init();
                    }
                    break;
            }

            createdEntity.SetPosition(pos);
            createdEntity.AddToBin(m_bin);

            return createdEntity;
        }

        public void Update(ref FrameTime frameTime, int updateToken)
        {
            UpdateEnemyPool(m_meleeEnemies, ref frameTime, updateToken);
            UpdateEnemyPool(m_misileEnemies, ref frameTime, updateToken);
        }

        private void UpdateEnemyPool(Pool<AiEntity> pool, ref FrameTime frameTime, int updateToken)
        {
            bool needsCoalesce = false;
            for (int i = 0; i < pool.ActiveItemListCount; ++i)
            {
                pool[i].Update(ref frameTime, updateToken + i);
                pool[i].UpdateBinEntry();

                pool[i].UpdateSensoryData(m_world.GetPlayerManager().GetPlayers().ActiveItemList);

                if (pool[i].IsDestroyed())
                {
                    m_bin.RemoveBinItem(pool[i], BinLayers.kAiEntity);
                    needsCoalesce = true;
                }
            }

            if (needsCoalesce)
            {
                pool.CoalesceActiveList(false);
            }
        }

        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_meleeEnemies.ActiveItemListCount; ++i)
            {
                m_meleeEnemies[i].DebugRender(debugRenderer);
            }
            for (int i = 0; i < m_misileEnemies.ActiveItemListCount; ++i)
            {
                m_misileEnemies[i].DebugRender(debugRenderer);
            }
        }

        public Pool<AiEntity> GetMeleeEnemies() { return m_meleeEnemies; }
        public Pool<AiEntity> GetMisileEnemies() { return m_misileEnemies; }

        public void IncrementKillCount() { ++m_killCounter; }
        public int GetKillCount() { return m_killCounter; }
    }
}
