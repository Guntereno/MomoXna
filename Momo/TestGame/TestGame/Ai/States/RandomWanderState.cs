using System;
using Microsoft.Xna.Framework;
using Momo.Core;
using TestGame.Entities;

namespace TestGame.Ai.States
{
    public class RandomWanderState : State
    {
        public RandomWanderState(AiEntity entity) :
            base(entity)
        { }

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

            GetEntity().DebugColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        }

        public override void Update(ref FrameTime frameTime)
        {
            GameWorld world = GetEntity().GetWorld();

            if (GetEntity().SensoryData.SensePlayer)
            {
                GetEntity().SetCurrentState(m_foundPlayerState);
            }
            else
            {
                Random random = world.GetRandom();

                float turnVelocity = GetEntity().GetTurnVelocity();
                turnVelocity += ((float)random.NextDouble() - 0.5f) * 50.0f * frameTime.Dt;
                turnVelocity = MathHelper.Clamp(turnVelocity, -1.0f, 1.0f);
                GetEntity().SetTurnVelocity(turnVelocity);

                float facingAngle = GetEntity().FacingAngle;
                facingAngle += turnVelocity * frameTime.Dt;
                GetEntity().FacingAngle = facingAngle;

                Vector2 direction = new Vector2((float)Math.Sin(facingAngle), (float)Math.Cos(facingAngle));
                Vector2 newPosition = GetEntity().GetPosition() + direction;

                GetEntity().SetPosition(newPosition);
            }
        }

        State m_foundPlayerState = null;
    }
}
