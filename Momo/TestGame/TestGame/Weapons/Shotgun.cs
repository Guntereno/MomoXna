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
        #region Fields

        ShotgunParams m_shotgunParams = null;

        ActiveState m_activeState = null;
        EmptyState m_emptyState = null;
        ReloadState m_reloadState = null;
        CoolDownState m_coolDownState = null;

        #endregion

        #region Constructor

        public Shotgun(Zone zone) : base(zone)
        {
            m_activeState = new ActiveState(this);
            m_emptyState = new EmptyState(this);
            m_reloadState = new ReloadState(this);
            m_coolDownState = new CoolDownState(this);

            m_activeState.EmptyState = m_emptyState;
            m_activeState.CoolDownState = m_coolDownState;

            m_emptyState.NextState = m_activeState;

            m_reloadState.NextState = m_activeState;

            m_coolDownState.NextState = m_activeState;
        }

        #endregion

        #region Public Interface

        public override bool AcceptingInput
        {
            get
            {
                return StateMachine.CurrentState == m_activeState;
            }
        }

        public override string ToString()
        {
            return "Shotgun";
        }

        public override void Init()
        {
            m_shotgunParams = kDefaultParams;
            mParams = m_shotgunParams;

            base.Init();

            StateMachine.CurrentState = m_activeState;
        }

        public override void Reload()
        {
            StateMachine.CurrentState = m_reloadState;
        }

        #endregion


        #region Params

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

        #endregion

        #region States

        public class ActiveState : WeaponState
        {
            #region Fields

            BulletEntity.BulletParams m_bulletParams = new BulletEntity.BulletParams(20.0f, new Color(0.9f, 0.6f, 0.1f, 0.4f));

            #endregion

            #region Constructor

            public ActiveState(Weapon weapon) :
                base(weapon)
            { }

            #endregion

            #region Public Interface

            public WeaponState CoolDownState { get; set; }
            public WeaponState EmptyState { get; set; }

            public override void Update(ref FrameTime frameTime, uint updateToken)
            {
                const float kTriggerThresh = 0.5f;
                if (Weapon.TriggerState > kTriggerThresh)
                {
                    int ammoInClip = Weapon.AmmoInClip;
                    if (ammoInClip > 0)
                    {
                        Zone zone = Weapon.Zone;

                        Random random = zone.Random;

                        ShotgunParams param = (ShotgunParams)(Weapon.Parameters);
                        for (int i = 0; i < param.m_shotCount; ++i)
                        {
                            float angle = Weapon.Facing + (((float)random.NextDouble() * param.m_spread) - (0.5f * param.m_spread));
                            Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));

                            Weapon.Recoil = -velocity * Weapon.Parameters.m_recoil;

                            velocity *= param.m_speed + (param.m_speed * ((float)random.NextDouble() * 0.08f));

                            zone.ProjectileManager.AddBullet(
                                Weapon.BarrelPosition,
                                velocity,
                                m_bulletParams,
                                Weapon.Owner.BulletGroupMembership);
                        }


                        --ammoInClip;
                        Weapon.AmmoInClip = ammoInClip;

                        zone.World.PlaySoundQueue("GUN_single2");

                        Weapon.StateMachine.CurrentState = CoolDownState;
                    }
                    else
                    {
                        Weapon.StateMachine.CurrentState = EmptyState;
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
