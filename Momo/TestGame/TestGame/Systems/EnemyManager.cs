using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.ObjectPools;
using Momo.Core.Spatial;
using Momo.Core;
using Momo.Debug;

using Game.Entities;
using Game.Entities.AI;



namespace Game.Systems
{
    public class AiEntityManager
    {
        private const int kMaxEntities = 1000;

        private Zone mZone;

        private Pool<AiEntity> mEntities = new Pool<AiEntity>(kMaxEntities, 2, 2, false);

        private int mKillCounter = 0;



        public Pool<AiEntity> Entities      { get { return mEntities; } }
        public int KillCount                { get { return mKillCounter; } }



        public AiEntityManager(Zone zone)
        {
            mZone = zone;

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
                mEntities.AddItem(new Civilian(mZone), false);
                mEntities.AddItem(new Zombie(mZone), false);
            }
        }


        // Not added to bins.
        public AiEntity Create(Type aiEntityType)
        {
            AiEntity createdEntity = null;

            createdEntity = mEntities.CreateItem(aiEntityType);

            return createdEntity;
        }


        public AiEntity Create(Type aiEntityType, Vector2 pos)
        {
            AiEntity createdEntity = Create(aiEntityType);

            createdEntity.SetPosition(pos);
            createdEntity.AddToBin();

            return createdEntity;
        }


        public void Update(ref FrameTime frameTime, uint updateToken)
        {
            UpdateEntityPool(mEntities, ref frameTime, updateToken);
        }


        public void PostUpdate()
        {
            mEntities.Update();
        }


        private void UpdateEntityPool(Pool<AiEntity> aiEntities, ref FrameTime frameTime, uint updateToken)
        {
            uint token = updateToken;
            for (int i = 0; i < aiEntities.ActiveItemListCount; ++i)
            {
                AiEntity aiEntity = aiEntities[i];
                aiEntity.Update(ref frameTime, token);
                aiEntity.UpdateBinEntry();
                ++token;
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
