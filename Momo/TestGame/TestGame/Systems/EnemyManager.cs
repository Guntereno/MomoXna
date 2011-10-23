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
        private const int kMaxEnemies = 500;

        private GameWorld m_world;
        private Bin m_bin;

        private Pool<EnemyEntity> m_enemies = new Pool<EnemyEntity>(kMaxEnemies, 2);

        private int m_killCounter = 0;



        public Pool<EnemyEntity> Enemies    { get { return m_enemies; } }
        public int KillCount                { get { return m_killCounter; } }



        public EnemyManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;

            m_enemies.RegisterPoolObjectType(typeof(MeleeEnemy), kMaxEnemies);
            m_enemies.RegisterPoolObjectType(typeof(MissileEnemy), kMaxEnemies);
        }


        public void IncrementKillCount()
        {
            ++m_killCounter;
        }


        public void Load()
        {
            for (int i = 0; i < kMaxEnemies; ++i)
            {
                m_enemies.AddItem(new MeleeEnemy(m_world), false);
                m_enemies.AddItem(new MissileEnemy(m_world), false);
            }
        }


        public EnemyEntity Create(MapData.EnemyData data, Vector2 pos)
        {
            EnemyEntity createdEntity = null;

            switch (data.GetSpecies())
            {
                case MapData.EnemyData.Species.Melee:
                    {
                        createdEntity = m_enemies.CreateItem(typeof(MeleeEnemy));
                        createdEntity.Init(data);
                    }
                    break;


                case MapData.EnemyData.Species.Missile:
                    {
                        createdEntity = m_enemies.CreateItem(typeof(MissileEnemy));
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
            UpdateEnemyPool(m_enemies, ref frameTime, updateToken);
        }


        private void UpdateEnemyPool(Pool<EnemyEntity> aiEntities, ref FrameTime frameTime, int updateToken)
        {
            bool needsCoalesce = false;
            for (int i = 0; i < aiEntities.ActiveItemListCount; ++i)
            {
                AiEntity aiEntity = aiEntities[i];
                aiEntity.Update(ref frameTime, updateToken + i);
                aiEntity.UpdateBinEntry();

                aiEntity.UpdateSensoryData(m_world.PlayerManager.Players.ActiveItemList);

                if (aiEntity.IsDestroyed())
                {
                    m_bin.RemoveBinItem(aiEntities[i], BinLayers.kAiEntity);
                    needsCoalesce = true;
                }
            }

            if (needsCoalesce)
            {
                aiEntities.CoalesceActiveList(false);
            }
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_enemies.ActiveItemListCount; ++i)
            {
                m_enemies[i].DebugRender(debugRenderer);
            }
        }
    }
}
