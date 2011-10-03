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
        public Minigun(GameWorld world): base(world)
        {
            m_activeState = new ActiveState(this);
            m_emptyState = new EmptyState(this);
            m_reloadState = new ReloadState(this);
            m_coolDownState = new CoolDownState(this);
            m_ventingHeatState = new VentHeat(this);

            m_activeState.Init(m_emptyState, m_coolDownState, m_ventingHeatState);
            m_emptyState.Init(m_activeState);
            m_reloadState.Init(m_activeState);
            m_coolDownState.Init(m_activeState);
            m_ventingHeatState.Init(m_activeState);
        }

        public override string ToString()
        {
            return "Minigun";
        }


        MinigunParams m_minigunParams;

        ActiveState m_activeState = null;
        EmptyState m_emptyState = null;
        ReloadState m_reloadState = null;
        CoolDownState m_coolDownState = null;
        VentHeat m_ventingHeatState = null;

        public float Heat { get; set; }


        public override void Init()
        {
            m_minigunParams = kDefaultParams;
            m_params = m_minigunParams;

            Heat = 0.0f;

            base.Init();

            SetCurrentState(m_activeState);
        }

        public override void Reload()
        {
            SetCurrentState(m_reloadState);
        }

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


        public class ActiveState : State
        {
            public ActiveState(Weapon weapon) :
                base(weapon)
            { }

            public override string ToString()
            {
                return "Active";
            }

            public void Init(State emptyState, State coolDownState, State overheatState)
            {
                m_emptyState = emptyState;
                m_coolDownState = coolDownState;
                m_overheatState = overheatState;
            }

            public override void Update(ref FrameTime frameTime)
            {
                Weapon weapon = GetWeapon();
                Minigun minigun = (Minigun)(weapon);

                const float kTriggerThresh = 0.5f;
                if (weapon.GetTriggerState() > kTriggerThresh)
                {
                    int ammoInClip = weapon.GetAmmoInClip();
                    if (ammoInClip > 0)
                    {
                        GameWorld world = weapon.GetWorld();

                        Random random = world.GetRandom();

                        MinigunParams param = (MinigunParams)(weapon.GetParams());
                        float angle = weapon.GetFacing() + (((float)random.NextDouble() * param.m_spread) - (0.5f * param.m_spread));

                        Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));

                        minigun.Recoil = -velocity * weapon.GetParams().m_recoil;

                        velocity *= param.m_speed;

                        world.GetProjectileManager().AddBullet(
                            weapon.GetBarrelPosition(),
                            velocity,
                            m_bulletParams,
                            weapon.GetOwner().GetBulletFlags());

                        --ammoInClip;
                        weapon.SetAmmoInClip(ammoInClip);

                        const float kHeatDelta = 0.4f;
                        minigun.Heat = minigun.Heat + kHeatDelta;

                        const float kOverheatThreshold = 50.0f;
                        if (minigun.Heat >= kOverheatThreshold)
                        {
                            weapon.SetCurrentState(m_overheatState);
                        }
                        else
                        {
                            weapon.SetCurrentState(m_coolDownState);
                        }
                    }
                    else
                    {
                        weapon.SetCurrentState(m_emptyState);
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

            public override bool AcceptingInput() { return true; }

            private State m_emptyState = null;
            private State m_coolDownState = null;
            private State m_overheatState = null;

            BulletEntity.Params m_bulletParams = new BulletEntity.Params(20.0f, new Color(0.9f, 0.8f, 0.6f, 0.4f));
        }

        public class VentHeat : State
        {
            public VentHeat(Weapon weapon) :
                base(weapon)
            { }

            public override string ToString()
            {
                return "Venting Heat";
            }

            public void Init(State nextState)
            {
                m_nextState = nextState;
            }

            public override void Update(ref FrameTime frameTime)
            {
                Weapon weapon = GetWeapon();
                Minigun minigun = (Minigun)(weapon);

                const float kCoolDelta = 0.4f;
                float heat = minigun.Heat;
                heat -= kCoolDelta;
                if (heat <= 0.0f)
                {
                    heat = 0.0f;
                    weapon.SetCurrentState(m_nextState);
                }
                minigun.Heat = heat;
            }

            public override bool AcceptingInput() { return false; }

            private State m_nextState = null;
        }

    }
}


