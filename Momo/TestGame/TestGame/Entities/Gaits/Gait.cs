using System;

using Microsoft.Xna.Framework;



namespace Game.Entities.Gaits
{
    public class Gait
    {

        protected float mLastMoveIntent = 0.0f;


        public float LastMoveIntent
        {
            get { return mLastMoveIntent; }
        }


        public virtual void WalkForward(GameEntity entity, float amount)
        {
            MoveForward(entity, amount);
        }

        public virtual void RunForward(GameEntity entity, float amount)
        {
            MoveForward(entity, amount);
        }

        protected void MoveForward(GameEntity entity, float amount)
        {
            mLastMoveIntent = amount;
            entity.IncrementPosition(entity.FacingDirection * amount);
        }
    }
}
