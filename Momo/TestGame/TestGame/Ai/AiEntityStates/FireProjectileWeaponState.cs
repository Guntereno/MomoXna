using TestGame.Entities;
using Momo.Core;
using Microsoft.Xna.Framework;
using TestGame.Weapons;
using TestGame.Ai.AiEntityStates;
using Momo.Core.StateMachine;

namespace TestGame.Ai.AiEntityStates
{
    public class FireProjectileWeaponState : AiEntityState
    {
        #region Constructor

        public FireProjectileWeaponState(AiEntity entity) :
            base(entity)
        {
        }

        #endregion

        #region Public Interface

        public State NoLongerInRangeState { get; set; }
        public State LostPlayerState  { get; set; }

        public RadiusInfo Range { get; set; }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            SensedObject sensedPlayer = AiEntity.SensoryData.SeePlayerSense;

            // I've we've lost the player
            if (sensedPlayer == null)
            {
                AiEntity.StateMachine.CurrentState = LostPlayerState;
                return;
            }

            // Check the range
            Vector2 offset = sensedPlayer.GetLastPosition() - AiEntity.GetPosition();
            if (offset.LengthSquared() >= Range.RadiusSq)
            {
                AiEntity.StateMachine.CurrentState = NoLongerInRangeState;
                return;
            }

            // Steer towards the player
            {
                Vector2 targetDirection = sensedPlayer.GetLastPosition() - AiEntity.GetPosition();
                targetDirection.Normalize();
                AiEntity.TurnTowards(targetDirection, 0.11f);
            }


            // Fire the weapon at the player
            Weapon weapon = AiEntity.CurrentWeapon;
            if (weapon != null)
            {
                // Reload if needed
                if (weapon.AmmoInClip == 0)
                {
                    weapon.Reload();
                }

                const float kFullPower = 1.0f;
                weapon.Update(ref frameTime, kFullPower, AiEntity.GetPosition(), AiEntity.FacingAngle);
            }
        }

        #endregion
    }
}
