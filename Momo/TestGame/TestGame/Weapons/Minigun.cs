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

            m_activeState.Init(m_emptyState, m_coolDownState);
            m_emptyState.Init(m_activeState);
            m_reloadState.Init(m_activeState);
            m_coolDownState.Init(m_activeState);
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


        public override void Init()
        {
            m_minigunParams = kDefaultParams;
            m_params = m_minigunParams;

            base.Init();

            SetCurrentState(m_activeState);
        }

        public override void Reload()
        {
            SetCurrentState(m_reloadState);
        }

        public class MinigunParams : Weapon.Params
        {
            public MinigunParams(float reloadTime, int clipSize, float velocity, float fireRate, float spread) :
                base(reloadTime, clipSize, velocity, fireRate)
            {
                m_spread = spread;
            }

            public float m_spread;
        }

        public static readonly MinigunParams kDefaultParams = new MinigunParams(4.0f, 600, 750.0f, 45.0f, 0.08f);


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

                        MinigunParams param = (MinigunParams)(GetWeapon().GetParams());
                        float angle = facing + (((float)random.NextDouble() * param.m_spread) - (0.5f * param.m_spread));
                        Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                        velocity *= param.m_velocity;

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

            BulletEntity.Params m_bulletParams = new BulletEntity.Params(20.0f, new Color(0.9f, 0.8f, 0.6f, 0.4f));
        }

    }
}


