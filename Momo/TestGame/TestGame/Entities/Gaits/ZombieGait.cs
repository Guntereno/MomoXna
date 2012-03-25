using System;

using Microsoft.Xna.Framework;



namespace Game.Entities.Gaits
{
    public class ZombieGait : Gait
    {
        float mStep = 0.0f;
        float mSway = 0.0f;

        float mSwayLeft = 0.05f;
        float mSwayRight = 0.25f;


        const float kWalkBobSpeed = 0.5f;
        const float kRunBobSpeed = 0.65f;
        const float kRunSwaySpeed = 0.125f;


        public float SwayLeft
        {
            set { mSwayLeft = value; }
        }

        public float SwayRight
        {
            set { mSwayRight = value; }
        }


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
            actualAmount = 0.10f + ((actualAmount + 1.0f) * amount * 0.90f);

            entity.IncrementPosition(entity.FacingDirection * actualAmount);

            entity.FacingVisualOffsetAngle *= 0.75f;
        }


        public override void RunForward(GameEntity entity, float amount)
        {
            float actualAmount = (float)Math.Sin(mStep);
            if (actualAmount > 0.0f)
            {
                mStep += amount * kRunBobSpeed;
            }
            else
            {
                mStep += amount * kRunBobSpeed * 0.2f;
            }
            actualAmount = 1.5f + ((actualAmount + 1.0f) * amount * 0.50f);

            entity.IncrementPosition(entity.FacingDirection * actualAmount);


            float swayAngle = (float)Math.Sin(mSway);

            mSway += amount * kRunSwaySpeed;
            if (swayAngle > 0.0f)
            {
                //mSway += amount * 0.075f;
                swayAngle *= mSwayLeft;
            }
            else
            {
                //mSway += amount * 0.20f;
                swayAngle *= mSwayRight;
            }

            entity.FacingVisualOffsetAngle = swayAngle;
        }

    }
}
