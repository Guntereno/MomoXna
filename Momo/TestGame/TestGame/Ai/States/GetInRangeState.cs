using TestGame.Entities;
using Momo.Core;
using Microsoft.Xna.Framework;

namespace TestGame.Ai.States
{
    class GetInRangeState : ChaseState
    {
        State m_inRangeState = null;
        RadiusInfo m_range = new RadiusInfo();

        public GetInRangeState(AiEntity entity) :
            base(entity)
        {
        }

        public void Init(State lostPlayerState, State inRangeState, float range)
        {
            base.Init(lostPlayerState);

            m_inRangeState = inRangeState;
            m_range.Radius = range;
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            // Follow the normal chase state
            base.Update(ref frameTime, updateToken);

            // Until the range is reached
            AiEntity entity = GetEntity();
            SensedObject sensedPlayer = entity.SensedPlayer;

            if (sensedPlayer != null)
            {
                Vector2 offset = sensedPlayer.GetLastPosition() - entity.GetPosition();
                if (offset.LengthSquared() <= m_range.RadiusSq)
                {
                    entity.SetCurrentState(m_inRangeState);
                }
            }
        }
    }
}
