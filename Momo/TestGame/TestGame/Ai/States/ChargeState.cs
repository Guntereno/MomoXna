using Microsoft.Xna.Framework;
using Momo.Core;
using TestGame.Entities;
using TestGame.Weapons;

namespace TestGame.Ai.States
{
    class ChargeState : ChaseState
    {
        public ChargeState(AiEntity entity) :
            base(entity)
        {
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
                DynamicGameEntity sensedEntity = sensedObject.SensedEntity;
                Weapon weapon = entity.GetCurrentWeapon();
                if ((weapon != null) && (sensedEntity != null))
                {
                    float distSq = (entity.GetPosition() - sensedEntity.GetPosition()).LengthSquared();

                    float totalRadii = entity.GetContactRadiusInfo().Radius + sensedEntity.GetContactRadiusInfo().Radius;
                    float totalRadiiSq = totalRadii * totalRadii;

                    const float kDistEpsilon = 1.0f;
                    const float kDistEpsilonSq = kDistEpsilon * kDistEpsilon;

                    float triggerAmount = 0.0f;
                    if ((distSq - totalRadiiSq) <= kDistEpsilonSq)
                    {
                        const float kFullPower = 1.0f;
                        triggerAmount = kFullPower;
                    }
                    weapon.Update(ref frameTime, triggerAmount, entity.GetPosition(), entity.FacingAngle);
                }
            }
        }
    }
}
