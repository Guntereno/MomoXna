using Momo.Core.ObjectPools;
using Momo.Core.Spatial;
using TestGame.Entities;
using Momo.Core;

namespace TestGame.Systems
{
    public class CorpseManager
    {
        private const int kMaxCorpses = 512;


        private GameWorld mWorld;

        private Pool<Corpse> mCorpses = new Pool<Corpse>(kMaxCorpses, 1, 2, false);


        public Pool<Corpse> Corpses { get { return mCorpses; } }


        public CorpseManager(GameWorld world)
        {
            mWorld = world;

            mCorpses.RegisterPoolObjectType(typeof(Corpse), kMaxCorpses);
        }

        public void Load()
        {
            for (int i = 0; i < kMaxCorpses; ++i)
            {
                mCorpses.AddItem(new Corpse(mWorld), false);
            }
        }

        public Corpse Create(AiEntity entity)
        {
            Corpse corpse = mCorpses.CreateItem(typeof(Corpse));

            corpse.AddToBin(mWorld.Bin);
            corpse.Init(entity);
            
            return corpse;
        }

        public void DebugRender(Momo.Debug.DebugRenderer debugRenderer)
        {
            for (int i = 0; i < mCorpses.ActiveItemListCount; ++i)
            {
                mCorpses[i].DebugRender(debugRenderer);
            }
        }

        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < mCorpses.ActiveItemListCount; ++i)
            {
                Corpse corpse = mCorpses[i];
                corpse.Update(ref frameTime);
            }
        }

        public void PostUpdate()
        {
            mCorpses.Update();
        }
    }
}
