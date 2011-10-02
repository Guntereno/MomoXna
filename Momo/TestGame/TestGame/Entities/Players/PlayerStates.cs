using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.StateMachine;
using Momo.Core;
using Microsoft.Xna.Framework;

namespace TestGame.Entities.Players
{
    internal class ActiveState : State
    {
        public ActiveState(StateMachine machine): base(machine)
        {
        }

        public override void OnEnter()
        {
            PlayerEntity player = (PlayerEntity)(GetMachine().GetOwner());

            player.DebugColor = player.GetPlayerColour();
        }

        public override void Update(ref FrameTime frameTime)
        {
            PlayerEntity player = (PlayerEntity)(GetMachine().GetOwner());

            player.UpdateInput();
            player.UpdateMovement(ref frameTime);
            player.UpdateWeapon(ref frameTime);
        }
    }

    internal class DyingState : TimedState
    {
        public DyingState(StateMachine machine)
            : base(machine)
        {
        }

        public override string ToString()
        {
            return "Dying (" + GetTimer().ToString("F3") + ")";
        }

        public override void OnEnter()
        {
            base.OnEnter();

            PlayerEntity player = (PlayerEntity)(GetMachine().GetOwner());

            Color color = Color.Gray;
            color.A = 128;
            player.DebugColor = color;
        }

        public override void OnExit()
        {
            PlayerEntity player = (PlayerEntity)(GetMachine().GetOwner());

            player.Kill();
        }
    }

    internal class DeadState : TimedState
    {
        public DeadState(StateMachine machine)
            : base(machine)
        {
        }

        public override string ToString()
        {
            return "Dead (" + GetTimer().ToString("F3") + ")";
        }


        public override void OnEnter()
        {
            base.OnEnter();

            PlayerEntity player = (PlayerEntity)(GetMachine().GetOwner());
            player.DebugColor = Color.Transparent;
        }

        public override void OnExit()
        {
            PlayerEntity player = (PlayerEntity)(GetMachine().GetOwner());

            player.Spawn();
        }
    }
}
