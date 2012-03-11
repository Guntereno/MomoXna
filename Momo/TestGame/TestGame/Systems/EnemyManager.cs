using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.ObjectPools;
using Momo.Core.Spatial;
using Momo.Core;
using Momo.Debug;

using TestGame.Entities;
using TestGame.Entities.AI;



namespace TestGame.Systems
{
    public class AiEntityManager
    {
        private const int kMaxEntities = 500;

        private GameWorld mWorld;
        private Bin mBin;

        private Pool<AiEntity> mEntities = new Pool<AiEntity>(kMaxEntities, 2);

        private int mKillCounter = 0;



        public Pool<AiEntity> Entities      { get { return mEntities; } }
        public int KillCount                { get { return mKillCounter; } }



        public AiEntityManager(GameWorld world, Bin bin)
        {
            mWorld = world;
            mBin = bin;

            mEntities.RegisterPoolObjectType(typeof(Civilian), kMaxEntities);
            mEntities.RegisterPoolObjectType(typeof(Zombie), kMaxEntities);
        }


        public void IncrementKillCount()
        {
            ++mKillCounter;
        }


        public void Load()
        {
            for (int i = 0; i < kMaxEntities; ++i)
            {
                mEntities.AddItem(new Civilian(mWorld), false);
                mEntities.AddItem(new Zombie(mWorld), false);
            }
        }


        public AiEntity Create(Type aiEntityType, Vector2 pos)
        {
            AiEntity createdEntity = null;

            createdEntity = mEntities.CreateItem(aiEntityType);
            createdEntity.SetPosition(pos);
            createdEntity.AddToBin(mBin);

            return createdEntity;
        }


        public void Update(ref FrameTime frameTime, uint updateToken)
        {
            UpdateEntityPool(mEntities, ref frameTime, updateToken);
        }


        private void UpdateEntityPool(Pool<AiEntity> aiEntities, ref FrameTime frameTime, uint updateToken)
        {
            bool needsCoalesce = false;
            uint token = updateToken;
            for (int i = 0; i < aiEntities.ActiveItemListCount; ++i)
            {
                AiEntity aiEntity = aiEntities[i];
                aiEntity.Update(ref frameTime, token);
                aiEntity.UpdateBinEntry();

                if (aiEntity.IsDestroyed())
                {
                    mBin.RemoveBinItem(aiEntities[i], BinLayers.kEnemyEntities);
                    needsCoalesce = true;
                }

                ++token;
            }

            if (needsCoalesce)
            {
                aiEntities.CoalesceActiveList(false);
            }
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < mEntities.ActiveItemListCount; ++i)
            {
                mEntities[i].DebugRender(debugRenderer);
            }
        }
    }
}
