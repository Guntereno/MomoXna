using System;

using Microsoft.Xna.Framework;



namespace Game.Entities.Gaits
{
    public class ZombieGait : Gait
    {
        float mStep = 0.0f;

        const float kWalkBobSpeed = 0.5f;


        public ZombieGait(float gaitStep)
        {
            mStep = gaitStep;
        }


        public override void WalkForward(GameEntity entity, float amount)
        {
            float actualAmount = (float)Math.Sin(mStep);
            if (actualAmount > 0.0f)
            {
                mStep += amount * kWalkBobSpeed;
            }
            else
            {
                mStep += amount * kWalkBobSpeed * 0.5f;
            }
            actualAmount = (actualAmount + 1.0f) * amount;

            entity.IncrementPosition(entity.FacingDirection * actualAmount);
        }


    }
}
