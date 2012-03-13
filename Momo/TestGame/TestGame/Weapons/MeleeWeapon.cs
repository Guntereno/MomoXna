using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;

namespace TestGame.Weapons
{
    public class MeleeWeapon : Weapon
    {
        #region Fields

        public static readonly GunParams kDefaultParams = new GunParams(0.0f, 0, 0.0f, 1.5f, 500.0f);
        
        private ActiveState m_activeState = null;
        private CoolDownState m_coolDownState = null;

        //private GunParams m_weaponParams = null;

        #endregion

        #region Constructor

        public MeleeWeapon(Zone zone)
            : base(zone)
        {
            m_activeState = new ActiveState(this);
            m_coolDownState = new CoolDownState(this);

            m_activeState.CoolDownState = m_coolDownState;
            m_coolDownState.NextState = m_activeState;
        }

        #endregion

        #region Public Interface

        public override bool AcceptingInput
        {
            get
            {
                return StateMachine.CurrentState == m_activeState;
            }
        }

        public override string ToString()
        {
            return "MeleeWeapon";
        }

        public override void Init()
        {
            mParams = kDefaultParams;

            base.Init();

            StateMachine.CurrentState = m_activeState;
        }

        public override void Reload()
        {
            // Nothing, no reloading needed
        }

        #endregion

        #region States

        public class ActiveState : WeaponState
        {
            #region Fields

            public ActiveState(Weapon weapon) :
                base(weapon)
            { }

            #endregion

            #region Public Interface

            public WeaponState CoolDownState { get; set; }

            public override void Update(ref FrameTime frameTime, uint updateToken)
            {
                const float kTriggerThresh = 0.5f;
                if (Weapon.TriggerState > kTriggerThresh)
                {
                    // Todo: Trigger an explosion at the barrel position

                    Weapon.StateMachine.CurrentState = CoolDownState;
                }
            }

            #endregion
        }

        #endregion
    }
}
