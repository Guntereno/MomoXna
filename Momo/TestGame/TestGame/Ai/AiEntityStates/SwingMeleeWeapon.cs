using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Core.StateMachine;
using TestGame.Entities;
using TestGame.Weapons;

namespace TestGame.Ai.AiEntityStates
{
    public class SwingMeleeWeapon : AiEntityState
    {
        private State m_chargeState = null;
        bool m_performedSwing = false;


        public SwingMeleeWeapon(AiEntity entity) :
            base(entity)
        {
        }

        public void Init(State chargeState)
        {
            m_chargeState = chargeState;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            m_performedSwing = false;
        }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            SensedObject sensedPlayer = AiEntity.SensoryData.SeePlayerSense;

            // Steer towards the player
            if(sensedPlayer != null)
            {
                Vector2 targetDirection = sensedPlayer.GetLastPosition() - AiEntity.GetPosition();
                targetDirection.Normalize();
                AiEntity.TurnTowards(targetDirection, 0.11f);
            }

            Weapon weapon = AiEntity.CurrentWeapon;

            float triggerAmount = 0.0f;
            if (!m_performedSwing)
            {
                if (weapon.AcceptingInput)
                {
                    const float kFullPower = 1.0f;
                    triggerAmount = kFullPower;
                    m_performedSwing = true;
                }
            }
            else
            {
                // Wait until it's active again
                if (weapon.AcceptingInput)
                {
                    AiEntity.StateMachine.CurrentState = m_chargeState;
                }
            }

            weapon.Update(ref frameTime, triggerAmount, AiEntity.GetPosition(), AiEntity.FacingAngle);
        }
    }
}
