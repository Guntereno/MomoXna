using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Pathfinding;
using Momo.Debug;

using TestGame.Entities;




namespace TestGame.Ai.States
{
    public class ChaseState : State
    {
        private State m_lostPlayerState = null;

        public ChaseState(AiEntity entity) :
            base(entity)
        {
        }

        public override string ToString()
        {
            return "Chase";
        }

        public void Init(State lostPlayerState)
        {
            m_lostPlayerState = lostPlayerState;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            GetEntity().DebugColor = new Color(0.875f, 0.05f, 0.05f, 0.5f);
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            AiEntity entity = GetEntity();

            SensedObject obj = null;

            if (entity.SensoryData.GetClosestSense(SensedType.kSeePlayer, ref obj))
            {
                Vector2 targetDirection = obj.GetLastPosition() - GetEntity().GetPosition();
                targetDirection.Normalize();

                GetEntity().TurnTowards(targetDirection, 0.11f);
                Vector2 newPosition = GetEntity().GetPosition() + GetEntity().FacingDirection * 2.0f;

                GetEntity().SetPosition(newPosition);
            }
            else
            {
                entity.SetCurrentState(m_lostPlayerState);
            }
        }


        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
