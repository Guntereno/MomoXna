using System;
using Microsoft.Xna.Framework;
using Momo.Core;
using TestGame.Entities;

namespace TestGame.Ai.States
{
    public class RandomWanderState : State
    {
        private float m_wanderTurnVelocity = 0.0f;

        private State m_foundPlayerState = null;


        public RandomWanderState(AiEntity entity) :
            base(entity)
        {
        
        }

        public override string ToString()
        {
            return "Random Wander";
        }

        public void Init(State foundPlayerState)
        {
            m_foundPlayerState = foundPlayerState;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            GetEntity().DebugColor = new Color(0.22f, 0.8f, 0.22f, 0.5f);
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            GameWorld world = GetEntity().GetWorld();

            if (GetEntity().SensoryData.SensePlayer)
            {
                GetEntity().SetCurrentState(m_foundPlayerState);
            }
            else
            {
                Random random = world.GetRandom();

                m_wanderTurnVelocity += ((float)random.NextDouble() - 0.5f) * 50.0f * frameTime.Dt;
                m_wanderTurnVelocity = MathHelper.Clamp(m_wanderTurnVelocity, -1.0f, 1.0f);

                float facingAngle = GetEntity().FacingAngle;
                facingAngle += m_wanderTurnVelocity * frameTime.Dt;
                GetEntity().FacingAngle = facingAngle;

                Vector2 direction = new Vector2((float)Math.Sin(facingAngle), (float)Math.Cos(facingAngle));
                Vector2 newPosition = GetEntity().GetPosition() + direction;

                GetEntity().SetPosition(newPosition);
            }
        }
    }
}
