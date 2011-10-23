using TestGame.Entities;
using Momo.Core;
using Microsoft.Xna.Framework;
using TestGame.Weapons;

namespace TestGame.Ai.States
{
    public class FireProjectileWeaponState : State
    {
        private State m_noLongerInRangeState = null;
        private State m_lostPlayerState = null;

        private RadiusInfo m_range = new RadiusInfo();

        public FireProjectileWeaponState(AiEntity entity) :
            base(entity)
        {
        }

        public void Init(State noLongerInRangeState, State lostPlayerState, float range)
        {
            m_noLongerInRangeState = noLongerInRangeState;
            m_lostPlayerState = lostPlayerState;
            m_range.Radius = range;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            GetEntity().PrimaryDebugColor = new Color(0.875f, 0.05f, 0.05f, 0.5f);
        }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            AiEntity entity = GetEntity();

            SensedObject sensedPlayer = entity.SensoryData.SeePlayerSense;

            // I've we've lost the player
            if (sensedPlayer == null)
            {
                entity.SetCurrentState(m_lostPlayerState);
                return;
            }

            // Check the range
            Vector2 offset = sensedPlayer.GetLastPosition() - entity.GetPosition();
            if (offset.LengthSquared() >= m_range.RadiusSq)
            {
                entity.SetCurrentState(m_noLongerInRangeState);
                return;
            }

            // Steer towards the player
            {
                Vector2 targetDirection = sensedPlayer.GetLastPosition() - entity.GetPosition();
                targetDirection.Normalize();
                GetEntity().TurnTowards(targetDirection, 0.11f);
            }


            // Fire the weapon at the player
            Weapon weapon = entity.CurrentWeapon;
            if (weapon != null)
            {
                // Reload if needed
                if (weapon.AmmoInClip == 0)
                {
                    weapon.Reload();
                }

                const float kFullPower = 1.0f;
                weapon.Update(ref frameTime, kFullPower, entity.GetPosition(), entity.FacingAngle);
            }
        }

    }
}
