using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;

namespace TestGame.Weapons
{
    public class MeleeWeapon : Weapon
    {
        ActiveState m_activeState = null;
        CoolDownState m_coolDownState = null;

        GunParams m_weaponParams = null;


        public static readonly GunParams kDefaultParams = new GunParams(0.0f, 0, 0.0f, 1.5f, 500.0f);


        public MeleeWeapon(GameWorld world)
            : base(world)
        {
            m_activeState = new ActiveState(this);
            m_coolDownState = new CoolDownState(this);

            m_activeState.Init(m_coolDownState);
            m_coolDownState.Init(m_activeState);
        }

        public override string ToString()
        {
            return "MeleeWeapon";
        }


        public override void Init()
        {
            m_params = kDefaultParams;

            base.Init();

            SetCurrentState(m_activeState);
        }

        public override void Reload()
        {
            // Nothing, no reloading needed
        }


        public class ActiveState : State
        {
            private State m_coolDownState = null;


            public ActiveState(Weapon weapon) :
                base(weapon)
            { }

            public override string ToString()
            {
                return "Active";
            }

            public void Init(State coolDownState)
            {
                m_coolDownState = coolDownState;
            }

            public override void Update(ref FrameTime frameTime)
            {
                Weapon weapon = GetWeapon();

                const float kTriggerThresh = 0.5f;
                if (weapon.GetTriggerState() > kTriggerThresh)
                {
                    // Trigger an explosion at the barrel position

                    weapon.SetCurrentState(m_coolDownState);
                }
            }

            public override bool AcceptingInput() { return true; }
        }
    }
}
