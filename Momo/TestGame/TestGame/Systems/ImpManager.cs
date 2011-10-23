using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.ObjectPools;
using Momo.Core.Spatial;
using Momo.Debug;

using TestGame.Entities;
using TestGame.Entities.Imps;



namespace TestGame.Systems
{
    public class ImpManager
    {
        private const int kMaxImps = 1000;

        private GameWorld m_world;
        private Bin m_bin;

        private Pool<Imp> m_imps = new Pool<Imp>(kMaxImps, 1);


        public Pool<Imp> Imps       { get { return m_imps; } }



        public ImpManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;

            m_imps.RegisterPoolObjectType(typeof(Imp), kMaxImps);
        }


        public void Load()
        {
            for (int i = 0; i < kMaxImps; ++i)
            {
                m_imps.AddItem(new Imp(m_world), false);
            }
        }


        public Imp Create(Vector2 pos)
        {
            Imp createdImp = null;

            createdImp = m_imps.CreateItem(typeof(Imp));
            createdImp.Init();

            createdImp.SetPosition(pos);
            createdImp.AddToBin(m_bin);

            return createdImp;
        }


        public void Update(ref FrameTime frameTime, int updateToken)
        {
            UpdateImpPool(m_imps, ref frameTime, updateToken);
        }


        private void UpdateImpPool(Pool<Imp> imps, ref FrameTime frameTime, int updateToken)
        {
            bool needsCoalesce = false;
            for (int i = 0; i < imps.ActiveItemListCount; ++i)
            {
                Imp imp = imps[i];
                imp.Update(ref frameTime, updateToken + i);
                imp.UpdateBinEntry();

                imp.UpdateSensoryData(m_world.PlayerManager.Players.ActiveItemList);

                if (imp.IsDestroyed())
                {
                    m_bin.RemoveBinItem(imp, BinLayers.kEnemyEntities);
                    needsCoalesce = true;
                }
            }

            if (needsCoalesce)
            {
                imps.CoalesceActiveList(false);
            }
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_imps.ActiveItemListCount; ++i)
            {
                m_imps[i].DebugRender(debugRenderer);
            }
        }
    }
}
