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

        static readonly int kMaxEnemies = 1000;
        private Pool<AiEntity> m_meleeEnemies = new Pool<AiEntity>(kMaxEnemies);
        private Pool<AiEntity> m_missileEnemies = new Pool<AiEntity>(kMaxEnemies);

        private int m_killCounter = 0;

        public EnemyManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;
        }

        public void Load()
        {
            for (int i = 0; i < kMaxEnemies; ++i)
            {
                m_meleeEnemies.AddItem(new MeleeEnemy(m_world), false);
                m_missileEnemies.AddItem(new MissileEnemy(m_world), false);
            }
        }


        public AiEntity Create(MapData.EnemyData data, Vector2 pos)
        {
            AiEntity createdEntity = null;

            switch (data.GetSpecies())
            {
                case MapData.EnemyData.Species.Melee:
                    {
                        createdEntity = m_meleeEnemies.CreateItem();
                        createdEntity.Init(data);
                    }
                    break;


                case MapData.EnemyData.Species.Missile:
                    {
                        createdEntity = m_missileEnemies.CreateItem();
                        createdEntity.Init(data);
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
            UpdateEnemyPool(m_missileEnemies, ref frameTime, updateToken);
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
            for (int i = 0; i < m_missileEnemies.ActiveItemListCount; ++i)
            {
                m_missileEnemies[i].DebugRender(debugRenderer);
            }
        }

        public Pool<AiEntity> GetMeleeEnemies() { return m_meleeEnemies; }
        public Pool<AiEntity> GetMisileEnemies() { return m_missileEnemies; }

        public void IncrementKillCount() { ++m_killCounter; }
        public int GetKillCount() { return m_killCounter; }
    }
}
