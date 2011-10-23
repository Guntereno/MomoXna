using Microsoft.Xna.Framework;
using Momo.Core;
using TestGame.Entities;
using TestGame.Weapons;

namespace TestGame.Ai.States
{
    public class SwingMeleeWeapon : State
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

            GetEntity().PrimaryDebugColor = new Color(0.875f, 0.05f, 0.05f, 0.5f);
            m_performedSwing = false;
        }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            AiEntity entity = GetEntity();

            SensedObject sensedPlayer = entity.SensoryData.SeePlayerSense;

            // Steer towards the player
            {
                Vector2 targetDirection = sensedPlayer.GetLastPosition() - entity.GetPosition();
                targetDirection.Normalize();
                GetEntity().TurnTowards(targetDirection, 0.11f);
            }

            Weapon weapon = entity.CurrentWeapon;

            float triggerAmount = 0.0f;
            if (!m_performedSwing)
            {
                if (weapon.AcceptingInput())
                {
                    const float kFullPower = 1.0f;
                    triggerAmount = kFullPower;
                    m_performedSwing = true;
                }
            }
            else
            {
                // Wait until it's active again
                if (weapon.AcceptingInput())
                {
                    entity.SetCurrentState(m_chargeState);
                }
            }

            weapon.Update(ref frameTime, triggerAmount, entity.GetPosition(), entity.FacingAngle);
        }
    }
}
