using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Momo.Core.ObjectPools;
using Momo.Core.Spatial;
using Momo.Core;
using Momo.Core.Models;
using Momo.Debug;

using Game.Entities;
using Game.Entities.AI;
using Momo.Core.Nodes.Cameras;



namespace Game.Systems
{
    public class AiEntityManager
    {
        private const int kMaxEntities = 1000;

        private Zone mZone = null;

        private Pool<AiEntity> mEntities = new Pool<AiEntity>(kMaxEntities, 2, 2, false);

        private InstancedModel mZombieInstancedModel = null;


        //private int mKillCounter = 0;



        public Pool<AiEntity> Entities      { get { return mEntities; } }
        //public int KillCount                { get { return mKillCounter; } }



        public AiEntityManager(Zone zone)
        {
            mZone = zone;

            mEntities.RegisterPoolObjectType(typeof(Civilian), kMaxEntities);
            mEntities.RegisterPoolObjectType(typeof(Zombie), kMaxEntities);


            Model zombieModel = Game.Instance.Content.Load<Model>("models/zombie");
            Effect instancedEffect = Game.Instance.Content.Load<Effect>("effects/instancedModel");
            Texture zombieTexture = Game.Instance.Content.Load<Texture>("textures/atlas");
            instancedEffect.Parameters["diffuseTex"].SetValue(zombieTexture);

            mZombieInstancedModel = new InstancedModel();
            mZombieInstancedModel.Init(kMaxEntities, zombieModel, instancedEffect, Game.Instance.GraphicsDevice);
        }


        //public void IncrementKillCount()
        //{
        //    ++mKillCounter;
        //}


        public void Load()
        {
            for (int i = 0; i < kMaxEntities; ++i)
            {
                Zombie zombie = new Zombie(mZone);
                zombie.InstanceModel = mZombieInstancedModel;

                Civilian civilian = new Civilian(mZone);

                mEntities.AddItem(zombie, false);
                mEntities.AddItem(civilian, false);
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


        public void Render(CameraNode camera, GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < mEntities.ActiveItemListCount; ++i)
            {
                mEntities[i].Render(camera, graphicsDevice);
            }

            mZombieInstancedModel.Render(camera.ViewProjectionMatrix, graphicsDevice);
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            //for (int i = 0; i < mEntities.ActiveItemListCount; ++i)
            //{
            //    mEntities[i].DebugRender(debugRenderer);
            //}
        }


        private void UpdateEntityPool(Pool<AiEntity> aiEntities, ref FrameTime frameTime, uint updateToken)
        {
            uint token = updateToken;
            for (int i = 0; i < aiEntities.ActiveItemListCount; ++i)
            {
                AiEntity aiEntity = aiEntities[i];

                if (!aiEntity.IsDestroyed())
                {
                    aiEntity.Update(ref frameTime, token);
                    aiEntity.UpdateBinEntry();
                }
                ++token;
            }
        }


    }
}
