using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Microsoft.Xna.Framework;
using TestGame.Entities;

namespace TestGame.Weapons
{
    class Minigun: Weapon
    {
        #region Fields

        MinigunParams m_minigunParams;

        ActiveState m_activeState = null;
        EmptyState m_emptyState = null;
        ReloadState m_reloadState = null;
        CoolDownState m_coolDownState = null;
        VentHeat m_ventingHeatState = null;

        #endregion

        #region Constructor

        public Minigun(GameWorld world): base(world)
        {
            m_activeState = new ActiveState(this);
            m_emptyState = new EmptyState(this);
            m_reloadState = new ReloadState(this);
            m_coolDownState = new CoolDownState(this);
            m_ventingHeatState = new VentHeat(this);

            m_activeState.EmptyState = m_emptyState;
            m_activeState.CoolDownState = m_coolDownState;
            m_activeState.OverheatState = m_ventingHeatState;
            
            m_emptyState.NextState = m_activeState;

            m_reloadState.NextState = m_activeState;

            m_coolDownState.NextState = m_activeState;

            m_ventingHeatState.NextState = m_activeState;
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
            return "Minigun";
        }

        public float Heat { get; set; }

        public override void Init()
        {
            m_minigunParams = kDefaultParams;
            m_params = m_minigunParams;

            Heat = 0.0f;

            base.Init();

            StateMachine.CurrentState = m_activeState;
        }

        public override void Reload()
        {
            StateMachine.CurrentState = m_reloadState;
        }

        #endregion

        #region Params

        public class MinigunParams : Weapon.GunParams
        {
            public MinigunParams(float reloadTime, int clipSize, float speed, float fireRate, float recoil, float spread) :
                base(reloadTime, clipSize, speed, fireRate, recoil)
            {
                m_spread = spread;
            }

            public float m_spread;
        }

        public static readonly MinigunParams kDefaultParams = new MinigunParams(4.0f, 600, 1400.0f, 45.0f, 23000.0f, 0.15f);

        #endregion

        #region States

        public class ActiveState : WeaponState
        {
            #region Fields

            BulletEntity.BulletParams m_bulletParams = new BulletEntity.BulletParams(20.0f, new Color(0.9f, 0.8f, 0.6f, 0.4f));

            #endregion

            #region Constructor

            public ActiveState(Weapon weapon) :
                base(weapon)
            { }

            #endregion

            #region Public Interface

            public WeaponState EmptyState { get; set; }
            public WeaponState CoolDownState { get; set; }
            public WeaponState OverheatState { get; set; }

            public override void Update(ref FrameTime frameTime, uint updateToken)
            {
                Minigun minigun = (Minigun)(Weapon);

                const float kTriggerThresh = 0.5f;
                if (Weapon.TriggerState > kTriggerThresh)
                {
                    int ammoInClip = Weapon.AmmoInClip;
                    if (ammoInClip > 0)
                    {
                        GameWorld world = Weapon.World;

                        Random random = world.Random;

                        MinigunParams param = (MinigunParams)(Weapon.Parameters);
                        float angle = Weapon.Facing + (((float)random.NextDouble() * param.m_spread) - (0.5f * param.m_spread));

                        Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));

                        minigun.Recoil = -velocity * Weapon.Parameters.m_recoil;

                        velocity *= param.m_speed;

                        world.ProjectileManager.AddBullet(
                            Weapon.BarrelPosition,
                            velocity,
                            m_bulletParams,
                            Weapon.Owner.BulletGroupMembership);

                        --ammoInClip;
                        Weapon.AmmoInClip = ammoInClip;

                        world.PlaySoundQueue("GUN_machine");

                        const float kHeatDelta = 0.4f;
                        minigun.Heat = minigun.Heat + kHeatDelta;

                        const float kOverheatThreshold = 50.0f;
                        if (minigun.Heat >= kOverheatThreshold)
                        {
                            Weapon.StateMachine.CurrentState = OverheatState;
                        }
                        else
                        {
                            Weapon.StateMachine.CurrentState = CoolDownState;
                        }
                    }
                    else
                    {
                        Weapon.StateMachine.CurrentState = EmptyState;
                    }
                }
                else
                {
                    // Cool down when not used
                    const float kCoolDelta = 0.2f;

                    float heat = minigun.Heat;
                    heat -= kCoolDelta;
                    if (heat < 0.0f)
                        heat = 0.0f;

                    minigun.Heat = heat;
                }
            }
            #endregion
        }

        public class VentHeat : WeaponState
        {
            #region Constructor

            public VentHeat(Weapon weapon) :
                base(weapon)
            { }

            #endregion

            #region Public Interface

            public WeaponState NextState { get; set; }

            public override void Update(ref FrameTime frameTime, uint updateToken)
            {
                Minigun minigun = (Minigun)(Weapon);

                const float kCoolDelta = 0.4f;
                float heat = minigun.Heat;
                heat -= kCoolDelta;
                if (heat <= 0.0f)
                {
                    heat = 0.0f;
                    Weapon.StateMachine.CurrentState = NextState;
                }
                minigun.Heat = heat;
            }

            #endregion
        }

        #endregion
    }
}


