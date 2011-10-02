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

            GetEntity().DebugColor = new Color(0.9f, 0.23f, 0.03f, 0.5f);
        }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            AiEntity entity = GetEntity();

            SensedObject obj = entity.SensedPlayer;
            if (obj != null)
            {
                Vector2 targetDirection = obj.GetLastPosition() - GetEntity().GetPosition();
                targetDirection.Normalize();

                GetEntity().TurnTowards(targetDirection, 0.05f);
                float speed = entity.GetRelativeFacing(targetDirection) * 1.8f;
                Vector2 newPosition = GetEntity().GetPosition() + GetEntity().FacingDirection * speed;

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
