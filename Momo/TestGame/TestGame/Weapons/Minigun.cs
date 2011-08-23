using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Microsoft.Xna.Framework;

namespace TestGame.Weapons
{
    class Minigun: Weapon
    {
        public Minigun(GameWorld world): base(world)
        {
        }

        public override void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing)
        {
            const float kTriggerThresh = 0.5f;
            if (triggerState > kTriggerThresh)
            {
                Random random = GetWorld().GetRandom();

                const float kVariance = 0.1f;
                float angle = facing + (((float)random.NextDouble() * kVariance) - (0.5f * kVariance));
                Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                velocity *= 750.0f;

                GetWorld().GetProjectileManager().AddBullet(pos, velocity);
            }
        }
    }
}
