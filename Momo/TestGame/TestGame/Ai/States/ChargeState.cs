using Microsoft.Xna.Framework;
using Momo.Core;
using TestGame.Entities;
using TestGame.Weapons;

namespace TestGame.Ai.States
{
    class ChargeState : ChaseState
    {
        private State m_attackState = null;

        public ChargeState(AiEntity entity) :
            base(entity)
        {
        }

        public void Init(State lostPlayerState, State attackState)
        {
            base.Init(lostPlayerState);

            m_attackState = attackState;
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            // Follow the normal chase state
            base.Update(ref frameTime, updateToken);

            // If touching the player, attack him
            AiEntity entity = GetEntity();
            SensedObject sensedObject = entity.SensoryData.SeePlayerSense;
            if (sensedObject != null)
            {
                GameEntity sensedEntity = sensedObject.SensedEntity;
                if (sensedEntity != null)
                {
                    float distSq = (entity.GetPosition() - sensedEntity.GetPosition()).LengthSquared();

                    float totalRadii = entity.ContactRadiusInfo.Radius + sensedEntity.ContactRadiusInfo.Radius;
                    float totalRadiiSq = totalRadii * totalRadii;

                    const float kDistEpsilon = 1.0f;
                    const float kDistEpsilonSq = kDistEpsilon * kDistEpsilon;

                    if ((distSq - totalRadiiSq) <= kDistEpsilonSq)
                    {
                        entity.SetCurrentState(m_attackState);
                    }
                }
            }
        }
    }
}
