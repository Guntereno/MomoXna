using System;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;



namespace TestGame.Entities.Players
{
    public partial class PlayerEntity
    {
        public abstract class PlayerEntityState : State
        {
            #region Constructor

            public PlayerEntityState(IStateMachineOwner owner)
                : base(owner)
            {
                DebugColor = Color.Magenta;
            }

            #endregion

            #region Public Interface

            public override string ToString()
            {
                return this.GetType().Name;
            }

            public Color DebugColor { get; set; }
            
            public PlayerEntity PlayerEntity
            {
                get { return Owner as PlayerEntity; }
            }

            public override void OnEnter()
            {
                base.OnEnter();

                PlayerEntity.PrimaryDebugColor = DebugColor;
            }

            #endregion
        }

        protected class TimedState : PlayerEntityState
        {
            #region Constructor

            public TimedState(IStateMachineOwner owner) :
                base(owner)
            {
            }

            #endregion


            #region Public Interface

            public override string ToString()
            {
                return this.GetType().Name + " (" + Timer.ToString("F3") + ")";
            }

            public float Timer{ get; private set; }
            public float Length{ get; set; }
            public State ExitState{ get; set; }

            public override void OnEnter()
            {
                base.OnEnter();

                Timer = 0.0f;
            }

            public override void Update(ref FrameTime frameTime, uint updateToken)
            {
                Timer += frameTime.Dt;
                if (Timer >= Length)
                {
                    Owner.StateMachine.CurrentState = ExitState;
                }
            }

            #endregion
        }

        protected class ActiveState : PlayerEntityState
        {
            public ActiveState(IStateMachineOwner owner)
                : base(owner)
            {
            }

            public override void Update(ref FrameTime frameTime, uint updateToken)
            {
                PlayerEntity.UpdateInput();
                PlayerEntity.UpdateMovement(ref frameTime);
                PlayerEntity.UpdateWeapon(ref frameTime);
            }
        }

        protected class DyingState : TimedState
        {
            public DyingState(IStateMachineOwner owner)
                : base(owner)
            {
            }

            public override void OnExit()
            {
                base.OnExit();

                PlayerEntity.Kill();
            }
        }

        protected class DeadState : TimedState
        {
            public DeadState(IStateMachineOwner owner)
                : base(owner)
            {
            }

            public override void OnExit()
            {
                PlayerEntity player = (PlayerEntity)(Owner);

                player.Spawn();

                // Set the gun index to 0. This is just to prove that 
                // a PlayerState can access private members of the player
                player.m_currentWeaponIdx = 0;
            }
        }

    }
}
