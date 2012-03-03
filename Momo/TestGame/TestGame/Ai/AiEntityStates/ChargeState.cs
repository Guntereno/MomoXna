using Microsoft.Xna.Framework;
using Momo.Core;
using TestGame.Entities;
using TestGame.Weapons;
using Momo.Core.StateMachine;

namespace TestGame.Ai.AiEntityStates
{
    class ChargeState : ChaseState
    {
        #region Constructor

        public ChargeState(AiEntity entity) :
            base(entity)
        {
        }

        #endregion

        #region Public Interface

        public State AttackState{ get; set; }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            // Follow the normal chase state
            base.Update(ref frameTime, updateToken);

            // If touching the player, attack him
            SensedObject sensedObject = AiEntity.SensoryData.SeePlayerSense;
            if (sensedObject != null)
            {
                GameEntity sensedEntity = sensedObject.SensedEntity;
                if (sensedEntity != null)
                {
                    float distSq = (AiEntity.GetPosition() - sensedEntity.GetPosition()).LengthSquared();

                    float totalRadii = AiEntity.ContactRadiusInfo.Radius + sensedEntity.ContactRadiusInfo.Radius;
                    float totalRadiiSq = totalRadii * totalRadii;

                    const float kDistEpsilon = 1.0f;
                    const float kDistEpsilonSq = kDistEpsilon * kDistEpsilon;

                    if ((distSq - totalRadiiSq) <= kDistEpsilonSq)
                    {
                        AiEntity.StateMachine.CurrentState = AttackState;
                    }
                }
            }
        }

        #endregion
    }
}
