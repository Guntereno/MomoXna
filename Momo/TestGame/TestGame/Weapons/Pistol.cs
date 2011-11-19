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
        public static readonly PistolParams kDefaultParams = new PistolParams(1.5f, 6, 1400.0f, 1.5f, 80000.0f);

        private PistolParams m_pistolParams = null;

        private EmptyState m_emptyState = null;
        private CoolDownState m_coolDownState = null;
        private ActiveState m_activeState = null;
        private ReloadState m_reloadState = null;



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


        public class PistolParams : Weapon.GunParams
        {
            public PistolParams(float reloadTime, int clipSize, float speed, float fireRate, float recoil):
                base(reloadTime, clipSize, speed, fireRate, recoil)
            {}
        }


        public class ActiveState : State
        {
            private BulletEntity.BulletParams m_bulletParams = new BulletEntity.BulletParams(100.0f, new Color(0.9f, 0.4f, 0.1f, 0.4f));

            private State m_emptyState = null;
            private State m_coolDownState = null;


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


            public override bool AcceptingInput() { return true; }

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

                        PistolParams param = (PistolParams)(weapon.Parameters);
                        weapon.Recoil = -(weapon.Direction) * weapon.Parameters.m_recoil;

                        Vector2 velocity = weapon.Direction * param.m_speed;

                        world.ProjectileManager.AddBullet(
                            weapon.BarrelPosition,
                            velocity,
                            m_bulletParams,
                            weapon.Owner.BulletGroupMembership);

                        --ammoInClip;
                        weapon.AmmoInClip = ammoInClip;

                        world.SoundBank.PlayCue("GUN_single");

                        weapon.SetCurrentState(m_coolDownState);
                    }
                    else
                    {
                        weapon.SetCurrentState(m_emptyState);
                    }
                }
            }
        }
    }
}
