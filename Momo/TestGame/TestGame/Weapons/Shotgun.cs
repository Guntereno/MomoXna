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
        public Shotgun(GameWorld world) : base(world)
        {
        }

        public class ShotgunParams : Weapon.Params
        {
            public ShotgunParams(float reloadTime, int clipSize, float velocity, float fireRate, float spread, int shotCount) :
                base(reloadTime, clipSize, velocity, fireRate)
            {
                m_spread = spread;
                m_shotCount = shotCount;
            }

            public float m_spread; // radians
            public int m_shotCount;
        }

        public static readonly ShotgunParams kDefaultParams = new ShotgunParams(1.5f, 10, 1200.0f, 1.5f, (float)(0.1f * Math.PI), 20);

        enum State
        {
            Active,
            Waiting,
            Reloading
        }

        public override void Init()
        {
            m_shotgunParams = kDefaultParams;
            m_params = m_shotgunParams;
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

                                for (int i = 0; i < m_shotgunParams.m_shotCount; ++i)
                                {
                                    float angle = facing + (((float)random.NextDouble() * m_shotgunParams.m_spread) - (0.5f * m_shotgunParams.m_spread));
                                    Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                                    velocity *= m_params.m_velocity;

                                    GetWorld().GetProjectileManager().AddBullet(pos, velocity);
                                }
                                --m_ammoInClip;

                                m_timer = 1.0f / m_params.m_fireRate;
                                m_state = State.Waiting;
                            }
                            else
                            {
                                m_timer = m_params.m_reloadTime;
                                m_state = State.Reloading;
                            }
                        }
                    }
                    break;

                case State.Waiting:
                    {
                        m_timer -= frameTime.Dt;
                        if (m_timer <= 0.0f)
                        {
                            m_state = State.Active;
                        }
                    }
                    break;

                case State.Reloading:
                    {
                        m_timer -= frameTime.Dt;
                        if (m_timer <= 0.0f)
                        {
                            m_ammoInClip = m_params.m_clipSize;
                            m_state = State.Active;
                        }
                    }
                    break;
            }

        }

        State m_state = State.Active;

        ShotgunParams m_shotgunParams = null;
        int m_ammoInClip = 0;
        float m_timer = 0.0f;
    }
}
