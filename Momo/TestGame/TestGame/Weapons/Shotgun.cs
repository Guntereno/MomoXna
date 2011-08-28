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
            m_activeState = new ActiveState(this);
            m_emptyState = new EmptyState(this);
            m_reloadState = new ReloadState(this);
            m_coolDownState = new CoolDownState(this);

            m_activeState.Init(m_emptyState, m_coolDownState);
            m_emptyState.Init(m_activeState);
            m_reloadState.Init(m_activeState);
            m_coolDownState.Init(m_activeState);
        }


        public override void Init()
        {
            m_shotgunParams = kDefaultParams;
            m_params = m_shotgunParams;

            base.Init();

            SetCurrentState(m_activeState);
        }

        public override void Reload()
        {
            SetCurrentState(m_reloadState);
        }

        ShotgunParams m_shotgunParams = null;

        ActiveState m_activeState = null;
        EmptyState m_emptyState = null;
        ReloadState m_reloadState = null;
        CoolDownState m_coolDownState = null;


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


        public class ActiveState : State
        {
            public ActiveState(Weapon weapon) :
                base(weapon)
            { }

            public override string ToString()
            {
                return "Active";
            }

            public void Init(State emptyState, State coolDownState)
            {
                m_emptyState = emptyState;
                m_coolDownState = coolDownState;
            }

            public override void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing)
            {
                Weapon weapon = GetWeapon();

                const float kTriggerThresh = 0.5f;
                if (triggerState > kTriggerThresh)
                {
                    int ammoInClip = weapon.GetAmmoInClip();
                    if (ammoInClip > 0)
                    {
                        GameWorld world = weapon.GetWorld();

                        Random random = world.GetRandom();

                        ShotgunParams param = (ShotgunParams)(GetWeapon().GetParams());
                        for (int i = 0; i < param.m_shotCount; ++i)
                        {
                            float angle = facing + (((float)random.NextDouble() * param.m_spread) - (0.5f * param.m_spread));
                            Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));

                            velocity *= param.m_velocity + (param.m_velocity * ((float)random.NextDouble() * 0.08f));

                            world.GetProjectileManager().AddBullet(pos, velocity);
                        }


                        --ammoInClip;
                        weapon.SetAmmoInClip(ammoInClip);

                        weapon.SetCurrentState(m_coolDownState);
                    }
                    else
                    {
                        weapon.SetCurrentState(m_emptyState);
                    }
                }
            }

            private State m_emptyState = null;
            private State m_coolDownState = null;
        }
    }
}
