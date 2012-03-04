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
        #region Fields

        public static readonly PistolParams kDefaultParams = new PistolParams(1.5f, 6, 1400.0f, 1.5f, 80000.0f);

        private PistolParams m_pistolParams = null;

        private EmptyState m_emptyState = null;
        private CoolDownState m_coolDownState = null;
        private ActiveState m_activeState = null;
        private ReloadState m_reloadState = null;

        #endregion

        #region Constructor

        public Pistol(GameWorld world) : base(world)
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
            return "Pistol";
        }


        public override void Init()
        {
            m_pistolParams = kDefaultParams;
            m_params = m_pistolParams;

            base.Init();

            StateMachine.CurrentState = m_activeState;
        }

        public override void Reload()
        {
            StateMachine.CurrentState = m_reloadState;
        }

        #endregion

        #region Params

        public class PistolParams : Weapon.GunParams
        {
            public PistolParams(float reloadTime, int clipSize, float speed, float fireRate, float recoil):
                base(reloadTime, clipSize, speed, fireRate, recoil)
            {}
        }

        #endregion

        #region States

        public class ActiveState : WeaponState
        {
            #region Fields

            private BulletEntity.BulletParams m_bulletParams = new BulletEntity.BulletParams(100.0f, new Color(0.9f, 0.4f, 0.1f, 0.4f));

            #endregion

            #region Constructor

            public ActiveState(Weapon weapon) :
                base(weapon)
            { }

            #endregion

            #region Public Interface

            public WeaponState EmptyState { get; set; }
            public WeaponState CoolDownState { get; set; }

            public override void Update(ref FrameTime frameTime, int updateToken)
            {
                const float kTriggerThresh = 0.5f;
                if (Weapon.TriggerState > kTriggerThresh)
                {
                    int ammoInClip = Weapon.AmmoInClip;
                    if (ammoInClip > 0)
                    {
                        GameWorld world = Weapon.World;

                        Random random = world.Random;

                        PistolParams param = (PistolParams)(Weapon.Parameters);
                        Weapon.Recoil = -(Weapon.Direction) * Weapon.Parameters.m_recoil;

                        Vector2 velocity = Weapon.Direction * param.m_speed;

                        world.ProjectileManager.AddBullet(
                            Weapon.BarrelPosition,
                            velocity,
                            m_bulletParams,
                            Weapon.Owner.BulletGroupMembership);

                        --ammoInClip;
                        Weapon.AmmoInClip = ammoInClip;

                        world.PlaySoundQueue("GUN_single");

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
