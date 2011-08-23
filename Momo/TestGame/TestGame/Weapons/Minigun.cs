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
            public Params(float reloadTime, int clipSize)
            {
                m_reloadTime = reloadTime; // seconds
                m_clipSize = clipSize;
            }

            public float m_reloadTime; // seconds
            public int m_clipSize;
        }

        public static readonly Params kDefaultParams = new Params(2.0f, 200);

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

                                const float kVariance = 0.1f;
                                float angle = facing + (((float)random.NextDouble() * kVariance) - (0.5f * kVariance));
                                Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                                velocity *= 750.0f;

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
