using Microsoft.Xna.Framework;
using Momo.Core;
using TestGame.Entities;

namespace TestGame.Ai.States
{
    public class ChaseState : State
    {
        public ChaseState(AiEntity entity) :
            base(entity)
        { }

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

            GetEntity().DebugColor = new Color(1.0f, 0.5f, 0.0f, 0.5f);
        }

        public override void Update(ref FrameTime frameTime)
        {
            AiEntity entity = GetEntity();

            if (entity.SensoryData.SensePlayer)
            {
                SensedObject obj = null;

                if (entity.SensoryData.GetClosestSense(SensedType.kSeePlayer, ref obj))
                {
                    Vector2 direction = obj.GetLastPosition() - GetEntity().GetPosition();
                    direction.Normalize();

                    GetEntity().FacingDirection = direction;

                    Vector2 newPosition = GetEntity().GetPosition() + direction;

                    GetEntity().SetPosition(newPosition);

                }
            }
            else
            {
                entity.SetCurrentState(m_lostPlayerState);
            }
        }

        State m_lostPlayerState = null;
    }
}
