using System;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;



namespace TestGame.Entities.Players
{
    public partial class PlayerEntity
    {
        protected class ActiveState : State
        {
            public ActiveState(IStateMachineOwner owner)
                : base(owner)
            {
            }

            public override void OnEnter()
            {
                PlayerEntity player = (PlayerEntity)(Owner);

                player.PrimaryDebugColor = player.PlayerColour;
            }

            public override void Update(ref FrameTime frameTime)
            {
                PlayerEntity player = (PlayerEntity)(Owner);

                player.UpdateInput();
                player.UpdateMovement(ref frameTime);
                player.UpdateWeapon(ref frameTime);
            }
        }

        protected class DyingState : TimedState
        {
            public DyingState(IStateMachineOwner owner)
                : base(owner)
            {
            }

            public override string ToString()
            {
                return "Dying (" + Timer.ToString("F3") + ")";
            }

            public override void OnEnter()
            {
                base.OnEnter();

                PlayerEntity player = (PlayerEntity)(Owner);

                Color color = Color.Gray;
                color.A = 128;
                player.PrimaryDebugColor = color;
            }

            public override void OnExit()
            {
                PlayerEntity player = (PlayerEntity)(Owner);

                player.Kill();
            }
        }

        protected class DeadState : TimedState
        {
            public DeadState(IStateMachineOwner owner)
                : base(owner)
            {
            }

            public override string ToString()
            {
                return "Dead (" + Timer.ToString("F3") + ")";
            }


            public override void OnEnter()
            {
                base.OnEnter();

                PlayerEntity player = (PlayerEntity)(Owner);
                player.PrimaryDebugColor = Color.Transparent;
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
