using System;
using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Core.StateMachine;
using TestGame.Entities;

namespace TestGame.Ai.AiEntityStates
{
    public class RandomWanderState : AiEntityState
    {
        #region Fields

        float m_wanderTurnVelocity = 0.0f;

        #endregion

        #region Constructor

        public RandomWanderState(AiEntity entity) :
            base(entity)
        {
        }

        #endregion

        #region Public Interface

        public State FoundPlayerState { get; set; }
        
        public override string ToString()
        {
            return "Random Wander";
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            GameWorld world = AiEntity.World;

            if (AiEntity.SensoryData.StraightPathToPlayerSense != null)
            {
                AiEntity.StateMachine.CurrentState = FoundPlayerState;
            }
            else
            {
                Random random = world.Random;

                m_wanderTurnVelocity += ((float)random.NextDouble() - 0.5f) * 50.0f * frameTime.Dt;
                m_wanderTurnVelocity = MathHelper.Clamp(m_wanderTurnVelocity, -1.0f, 1.0f);

                float facingAngle = AiEntity.FacingAngle;
                facingAngle += m_wanderTurnVelocity * frameTime.Dt;
                AiEntity.FacingAngle = facingAngle;

                Vector2 direction = new Vector2((float)Math.Sin(facingAngle), (float)Math.Cos(facingAngle));
                Vector2 newPosition = AiEntity.GetPosition() + direction;

                AiEntity.SetPosition(newPosition);
            }
        }

        #endregion
    }
}
