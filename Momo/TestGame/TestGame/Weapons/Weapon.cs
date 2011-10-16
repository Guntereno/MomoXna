using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Momo.Core.ObjectPools;
using Microsoft.Xna.Framework;

namespace TestGame.Weapons
{
    public abstract class Weapon : IPoolItem
    {
        protected bool m_isDestroyed = true;
        protected GunParams m_params = null;

        private GameWorld m_world = null;
        private State m_currentState = null;
        private int m_ammoInClip = 0;

        private Vector2 m_position = new Vector2();
        private Vector2 m_barrelPos = new Vector2();
        private Vector2 m_direction = new Vector2();
        private float m_facing = 0.0f;

        float m_triggerState = 0.0f;

        IWeaponUser m_owner = null;

        public bool AcceptingInput()
        {
            if (m_currentState == null)
            {
                return false;
            }
            else
            {
                return m_currentState.AcceptingInput();
            }
        }

        public class GunParams
        {
            public GunParams(float reloadTime, int clipSize, float speed, float fireRate, float recoil)
            {
                m_reloadTime = reloadTime;
                m_clipSize = clipSize;
                m_speed = speed;
                m_fireRate = fireRate;
                m_recoil = recoil;
            }

            public float m_reloadTime; // seconds
            public int m_clipSize;
            public float m_speed;
            public float m_fireRate; // shells/sec
            public float m_recoil;
        }

        public Weapon(GameWorld world)
        {
            m_world = world;
        }

        public override string ToString()
        {
            return "";
        }

        public Vector2 Recoil { get; set; }

        public GameWorld GetWorld() { return m_world; }
        public GunParams GetParams() { return m_params; }

        public IWeaponUser GetOwner() { return m_owner; }
        public void SetOwner(IWeaponUser owner) { m_owner = owner; }

        public int GetAmmoInClip() { return m_ammoInClip; }
        public void SetAmmoInClip(int ammo) { m_ammoInClip = ammo; }

        public Vector2 GetPosition() { return m_position; }
        public Vector2 GetBarrelPosition() { return m_barrelPos; }
        public Vector2 GetDirection() { return m_direction; }
        public float GetFacing() { return m_facing; }

        public float GetTriggerState() { return m_triggerState; }

        public void SetCurrentState(State state)
        {
            if (m_currentState != null)
            {
                m_currentState.OnExit();
            }

            m_currentState = state;

            if (m_currentState != null)
            {
                m_currentState.OnEnter();
            }
        }

        public String GetCurrentStateName()
        {
            if (m_currentState == null)
            {
                return "";
            }

            return m_currentState.ToString();
        }

        public virtual void Init()
        {
            m_ammoInClip = m_params.m_clipSize;
        }

        public void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing)
        {
            Recoil = Vector2.Zero;

            m_position = pos;
            m_facing = facing;
            m_direction = new Vector2((float)Math.Sin(facing), (float)Math.Cos(facing));

            m_triggerState = triggerState;

            // This should probably be a weapon parameter
            const float kWeaponLength = 20.0f;
            m_barrelPos = pos + (m_direction * kWeaponLength);

            if (m_currentState != null)
            {
                m_currentState.Update(ref frameTime);
            }
        }

        public virtual void Reload()
        {
            m_ammoInClip = m_params.m_clipSize;
        }

        public bool IsDestroyed()
        {
            return m_isDestroyed;
        }

        public void DestroyItem()
        {
            m_isDestroyed = true;
            GetWorld().WeaponManager.TriggerCoalesce();
        }

        public void ResetItem()
        {
            m_isDestroyed = false;
        }

        #region State classes

        public abstract class State
        {
            public State(Weapon weapon)
            {
                m_weapon = weapon;
            }

            public override string ToString()
            {
                return "";
            }

            public Weapon GetWeapon() { return m_weapon; }

            public virtual void OnEnter() {}
            public abstract void Update(ref FrameTime frameTime);
            public virtual void OnExit() {}

            public abstract bool AcceptingInput();

            Weapon m_weapon;
        }

        public abstract class TimedState : State
        {
            public TimedState(Weapon weapon) :
                base(weapon)
            { }

            public void Init(State nextState)
            {
                m_nextState = nextState;
            }

            protected abstract float GetTime();

            public override void OnEnter()
            {
                m_timer = GetTime();
            }

            public override void Update(ref FrameTime frameTime)
            {
                m_timer -= frameTime.Dt;
                if (m_timer <= 0.0f)
                {
                    GetWeapon().SetCurrentState(m_nextState);
                }
            }

            private State m_nextState = null;
            protected float m_timer;
        }

        public class ReloadState : TimedState
        {
            public ReloadState(Weapon weapon) :
                base(weapon)
            {}

            public override string ToString()
            {
                return "Reloading (" + m_timer.ToString("F3") + ")";
            }

            protected override float GetTime()
            {
                return GetWeapon().m_params.m_reloadTime;
            }

            public override void OnExit()
            {
                GunParams param = GetWeapon().GetParams();
                GetWeapon().SetAmmoInClip(param.m_clipSize);
            }

            public override bool AcceptingInput() { return false; }
        }


        public class CoolDownState : TimedState
        {
            public CoolDownState(Weapon weapon) :
                base(weapon)
            {}

            public override string ToString()
            {
                return "Cooldown  (" + m_timer.ToString("F3") + ")";
            }

            protected override float GetTime()
            {
                return 1.0f / GetWeapon().GetParams().m_fireRate;
            }

            public override bool AcceptingInput() { return false; }
        }

        public class EmptyState : State
        {
            public EmptyState(Weapon weapon) :
                base(weapon)
            { }

            public override string ToString()
            {
                return "Empty";
            }

            public void Init(State nextState)
            {
                m_nextState = nextState;
            }

            public override void Update(ref FrameTime frameTime)
            {
                // Empty is a dead-end state. Must be exited using a reload call
            }

            public override bool AcceptingInput() { return false; }

            private State m_nextState = null;
        }


        #endregion

    }
}
