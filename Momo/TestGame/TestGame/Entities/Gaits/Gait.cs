using System;

using Microsoft.Xna.Framework;



namespace Game.Entities.Gaits
{
    public class Gait
    {

        public virtual void WalkForward(GameEntity entity, float amount)
        {
            entity.IncrementPosition(entity.FacingDirection * amount);
        }

        public virtual void RunForward(GameEntity entity, float amount)
        {
            entity.IncrementPosition(entity.FacingDirection * amount);
        }
    }
}
