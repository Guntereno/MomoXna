using Momo.Core.ObjectPools;
using Momo.Core.Spatial;
using TestGame.Entities;
using Momo.Core;

namespace TestGame.Systems
{
    public class CorpseManager
    {
        private GameWorld m_world;
        private Bin m_bin;

        static readonly int kMaxCorpses = 512;

        private Pool<Corpse> m_corpses = new Pool<Corpse>(kMaxCorpses, 1);


        public Pool<Corpse> GetCorpses() { return m_corpses; }


        public CorpseManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;

            m_corpses.RegisterPoolObjectType(typeof(Corpse), kMaxCorpses);
        }

        public void Load()
        {
            for (int i = 0; i < kMaxCorpses; ++i)
            {
                m_corpses.AddItem(new Corpse(m_world), false);
            }
        }

        public Corpse Create(AiEntity entity)
        {
            Corpse corpse = m_corpses.CreateItem(typeof(Corpse));

            corpse.AddToBin(m_bin);
            corpse.Init(entity);
            
            return corpse;
        }

        public void DebugRender(Momo.Debug.DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_corpses.ActiveItemListCount; ++i)
            {
                m_corpses[i].DebugRender(debugRenderer);
            }
        }

        public void Update(ref FrameTime frameTime)
        {
            bool needsCoalesce = false;
            for (int i = 0; i < m_corpses.ActiveItemListCount; ++i)
            {
                m_corpses[i].Update(ref frameTime);

                if (m_corpses[i].IsDestroyed())
                {
                    m_bin.RemoveBinItem(m_corpses[i], BinLayers.kAiEntity);
                    needsCoalesce = true;
                }
            }

            if (needsCoalesce)
            {
                m_corpses.CoalesceActiveList(false);
            }
        }
    }
}
