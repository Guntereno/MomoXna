using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Microsoft.Xna.Framework;
using TestGame.Entities;

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

        public override string ToString()
        {
            return "Shotgun";
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


        public class ShotgunParams : Weapon.GunParams
        {
            public ShotgunParams(float reloadTime, int clipSize, float speed, float fireRate, float recoil, float spread, int shotCount) :
                base(reloadTime, clipSize, speed, fireRate, recoil)
            {
                m_spread = spread;
                m_shotCount = shotCount;
            }

            public float m_spread; // radians
            public int m_shotCount;
        }

        public static readonly ShotgunParams kDefaultParams = new ShotgunParams(2.5f, 10, 1300.0f, 1.5f, 80000.0f, (float)(0.1f * Math.PI), 10);


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

            public override void Update(ref FrameTime frameTime)
            {
                Weapon weapon = GetWeapon();

                const float kTriggerThresh = 0.5f;
                if (weapon.TriggerState > kTriggerThresh)
                {
                    int ammoInClip = weapon.AmmoInClip;
                    if (ammoInClip > 0)
                    {
                        GameWorld world = weapon.World;

                        Random random = world.Random;

                        ShotgunParams param = (ShotgunParams)(weapon.Parameters);
                        for (int i = 0; i < param.m_shotCount; ++i)
                        {
                            float angle = weapon.Facing + (((float)random.NextDouble() * param.m_spread) - (0.5f * param.m_spread));
                            Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));

                            weapon.Recoil = -velocity * weapon.Parameters.m_recoil;

                            velocity *= param.m_speed + (param.m_speed * ((float)random.NextDouble() * 0.08f));

                            world.ProjectileManager.AddBullet(
                                weapon.BarrelPosition,
                                velocity,
                                m_bulletParams,
                                weapon.Owner.BulletGroupMembership);
                        }


                        --ammoInClip;
                        weapon.AmmoInClip = ammoInClip;

                        weapon.SetCurrentState(m_coolDownState);
                    }
                    else
                    {
                        weapon.SetCurrentState(m_emptyState);
                    }
                }
            }

            public override bool AcceptingInput() { return true; }

            private State m_emptyState = null;
            private State m_coolDownState = null;

            BulletEntity.BulletParams m_bulletParams = new BulletEntity.BulletParams(20.0f, new Color(0.9f, 0.6f, 0.1f, 0.4f));
        }
    }
}
