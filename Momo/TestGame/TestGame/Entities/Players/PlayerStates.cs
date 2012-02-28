using System;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;



namespace TestGame.Entities.Players
{
    internal class ActiveState : State
    {
        public ActiveState(IStateMachineOwner owner): base(owner)
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

    internal class DyingState : TimedState
    {
        public DyingState(IStateMachineOwner owner)
            : base(owner)
        {
        }

        public override string ToString()
        {
            return "Dying (" + GetTimer().ToString("F3") + ")";
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

    internal class DeadState : TimedState
    {
        public DeadState(IStateMachineOwner owner)
            : base(owner)
        {
        }

        public override string ToString()
        {
            return "Dead (" + GetTimer().ToString("F3") + ")";
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
        }
    }
}
