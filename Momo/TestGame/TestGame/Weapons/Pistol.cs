using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Momo.Core;
using TestGame.Entities;

namespace TestGame.Weapons
{
    public class Pistol : Weapon
    {
        public Pistol(GameWorld world) : base(world)
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

        public override string ToString()
        {
            return "Pistol";
        }


        public override void Init()
        {
            m_pistolParams = kDefaultParams;
            m_params = m_pistolParams;

            base.Init();

            SetCurrentState(m_activeState);
        }

        public override void Reload()
        {
            SetCurrentState(m_reloadState);
        }

        PistolParams m_pistolParams = null;

        ActiveState m_activeState = null;
        EmptyState m_emptyState = null;
        ReloadState m_reloadState = null;
        CoolDownState m_coolDownState = null;


        public class PistolParams : Weapon.Params
        {
            public PistolParams(float reloadTime, int clipSize, float speed, float fireRate, float recoil):
                base(reloadTime, clipSize, speed, fireRate, recoil)
            {}
        }

        public static readonly PistolParams kDefaultParams = new PistolParams(1.5f, 6, 1400.0f, 1.5f, 80000.0f);


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

                        PistolParams param = (PistolParams)(GetWeapon().GetParams());
                        Vector2 velocity = new Vector2((float)Math.Sin(facing), (float)Math.Cos(facing));

                        weapon.Recoil = -velocity * weapon.GetParams().m_recoil;
                        
                        velocity *= param.m_speed;

                        world.GetProjectileManager().AddBullet(pos, velocity, m_bulletParams);

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

            BulletEntity.Params m_bulletParams = new BulletEntity.Params(100.0f, new Color(0.9f, 0.4f, 0.1f, 0.4f));
        }
    }
}
