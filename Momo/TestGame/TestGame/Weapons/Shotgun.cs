using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Microsoft.Xna.Framework;

namespace TestGame.Weapons
{
    public class Shotgun : Weapon
    {
        public Shotgun(TestWorld world) : base(world)
        {
        }

        public override void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing)
        {
        }
    }
}
