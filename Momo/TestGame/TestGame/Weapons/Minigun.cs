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

        public struct Params
        {
            public Params(float reloadTime, int clipSize, float velocity, float spread)
            {
                m_reloadTime = reloadTime;
                m_clipSize = clipSize;
                m_velocity = velocity;
                m_spread = spread;
            }

            public float m_reloadTime; // seconds
            public int m_clipSize;
            public float m_velocity;
            public float m_spread;
        }

        public static readonly Params kDefaultParams = new Params(4.0f, 1300, 750.0f, 0.08f);

        enum State
        {
            Active,
            Reloading
        }

        public override void Init()
        {
            m_params = kDefaultParams;
            m_state = State.Active;
            m_ammoInClip = m_params.m_clipSize;
        }

        public override void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing)
        {
            switch (m_state)
            {
                case State.Active:
                    {
                        const float kTriggerThresh = 0.5f;
                        if (triggerState > kTriggerThresh)
                        {
                            if (m_ammoInClip > 0)
                            {
                                Random random = GetWorld().GetRandom();

                                float angle = facing + (((float)random.NextDouble() * m_params.m_spread) - (0.5f * m_params.m_spread));
                                Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                                velocity *= m_params.m_velocity;

                                GetWorld().GetProjectileManager().AddBullet(pos, velocity);

                                --m_ammoInClip;
                            }
                            else
                            {
                                m_reloadTimer = m_params.m_reloadTime;
                                m_state = State.Reloading;
                            }
                        }
                    }
                    break;

                case State.Reloading:
                    {
                        m_reloadTimer -= frameTime.Dt;
                        if (m_reloadTimer <= 0.0f)
                        {
                            m_ammoInClip = m_params.m_clipSize;
                            m_state = State.Active;
                        }
                    }
                    break;
            }
        }

        State m_state = State.Active;
        Params m_params;

        int m_ammoInClip = 0;
        float m_reloadTimer;
    }
}
