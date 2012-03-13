using Momo.Core;
using Momo.Core.Spatial;

namespace TestGame.Entities
{
    public class Corpse : GameEntity
    {
        private float mMeat = 0.0f;
        private float mAge = 0.0f;


        public Corpse(Zone zone)
            : base(zone)
        {
            mBinLayer = BinLayers.kCorpse;
        }

        public void Init(AiEntity entity)
        {
            // Const for now. Should be a parameter of the enemy
            const float kMeatAmount = 100.0f;
            mMeat = kMeatAmount;

            ContactRadiusInfo = entity.ContactRadiusInfo;
            SetPosition(entity.GetPosition());

            // Initialise the bin
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Zone.Bin.GetBinRegionFromCentre(GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, ref curBinRegion);
            SetBinRegion(curBinRegion);
        }

        public override void ResetItem()
        {
            base.ResetItem();

            mMeat = 0.0f;
            mAge = 0.0f;
        }

        public void SetMeat(float meat)
        {
            mMeat = meat;
        }

        public void Update(ref FrameTime frameTime)
        {
            // Have we been harvested?
            if (mMeat <= 0.0f)
            {
                Perish();
            }
            else
            {
                mAge += frameTime.Dt;

                const float kLifeSpan = 20.0f;
                if (mAge > kLifeSpan)
                {
                    Perish();
                }
            }
        }

        public float HarvestMeat(float amount)
        {
            float harvested = 0.0f;
            if (mMeat > amount)
            {
                harvested = amount;
            }
            else
            {
                harvested = mMeat;
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
