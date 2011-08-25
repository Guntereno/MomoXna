using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Momo.Core;

namespace TestGame.Weapons
{
    public class Pistol : Weapon
    {
        public Pistol(GameWorld world) : base(world)
        {
        }

        public class PistolParams : Weapon.Params
        {
            public PistolParams(float reloadTime, int clipSize, float velocity, float fireRate):
                base(reloadTime, clipSize, velocity, fireRate)
            {}
        }

        public static readonly PistolParams kDefaultParams = new PistolParams(0.5f, 16, 1100.0f, 2.0f);

        enum State
        {
            Active,
            Waiting,
            Reloading
        }

        public override void Init()
        {
            m_pistolParams = kDefaultParams;
            m_params = m_pistolParams;
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
                                Vector2 velocity = new Vector2((float)Math.Sin(facing), (float)Math.Cos(facing));
                                velocity *= m_params.m_velocity;

                                GetWorld().GetProjectileManager().AddBullet(pos, velocity);

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
        PistolParams m_pistolParams = null;

        int m_ammoInClip = 0;
        float m_timer;
    }
}
