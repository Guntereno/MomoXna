using System;

using Microsoft.Xna.Framework;



namespace TestGame.Entities.Gaits
{
    public class Gait
    {

        public virtual void WalkForward(GameEntity entity, float amount)
        {
            entity.IncrementPosition(entity.FacingDirection * amount);
        }

    }
}
