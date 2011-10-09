using Momo.Core;
using Momo.Core.Spatial;

namespace TestGame.Entities
{
    public class Corpse : StaticGameEntity
    {
        float m_meat = 0.0f;
        float m_age = 0.0f;


        public Corpse(GameWorld world): base(world)
        {
        }

        public void Init(AiEntity entity)
        {
            // Const for now. Should be a parameter of the enemy
            const float kMeatAmount = 100.0f;
            m_meat = kMeatAmount;

            SetContactRadiusInfo(entity.GetContactRadiusInfo());
            SetPosition(entity.GetPosition());

            // Initialise the bin
            Bin bin = GetBin();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), ref curBinRegion);
            SetBinRegion(curBinRegion);
        }

        protected override void Reset()
        {
            base.Reset();

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
            AddToBin(bin, GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), BinLayers.kAiEntity);
        }

    }
}
