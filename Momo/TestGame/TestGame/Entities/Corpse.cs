using Momo.Core;
using Momo.Core.Spatial;

namespace TestGame.Entities
{
    public class Corpse : GameEntity
    {
        private float m_meat = 0.0f;
        private float m_age = 0.0f;


        public Corpse(GameWorld world)
            : base(world)
        {
            mBinLayer = BinLayers.kCorpse;
        }

        public void Init(AiEntity entity)
        {
            // Const for now. Should be a parameter of the enemy
            const float kMeatAmount = 100.0f;
            m_meat = kMeatAmount;

            ContactRadiusInfo = entity.ContactRadiusInfo;
            SetPosition(entity.GetPosition());

            // Initialise the bin
            BinRegionUniform curBinRegion = new BinRegionUniform();
            World.Bin.GetBinRegionFromCentre(GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, ref curBinRegion);
            SetBinRegion(curBinRegion);
        }

        public override void ResetItem()
        {
            base.ResetItem();

            m_meat = 0.0f;
            m_age = 0.0f;
        }

        public void SetMeat(float meat)
        {
            m_meat = meat;
        }

        public void Update(ref FrameTime frameTime)
        {
            // Have we been harvested?
            if (m_meat <= 0.0f)
            {
                Perish();
            }
            else
            {
                m_age += frameTime.Dt;

                const float kLifeSpan = 20.0f;
                if (m_age > kLifeSpan)
                {
                    Perish();
                }
            }
        }

        public float HarvestMeat(float amount)
        {
            float harvested = 0.0f;
            if (m_meat > amount)
            {
                harvested = amount;
            }
            else
            {
                harvested = m_meat;
            }

            return harvested;
        }

        private void Perish()
        {
            DestroyItem();
        }

        public void AddToBin(Bin bin)
        {
            AddToBin(bin, GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, mBinLayer);
        }

    }
}
