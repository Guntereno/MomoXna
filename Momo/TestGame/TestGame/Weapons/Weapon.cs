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
        public class Params
        {
            public Params(float reloadTime, int clipSize, float velocity, float fireRate)
            {
                m_reloadTime = reloadTime;
                m_clipSize = clipSize;
                m_velocity = velocity;
                m_fireRate = fireRate;
            }

            public float m_reloadTime; // seconds
            public int m_clipSize;
            public float m_velocity;
            public float m_fireRate; // shells/sec
        }

        public Weapon(GameWorld world)
        {
            m_world = world;
        }

        public GameWorld GetWorld() { return m_world; }
        public Params GetParams() { return m_params; }

        public int GetAmmoInClip() { return m_ammoInClip; }
        public void SetAmmoInClip(int ammo) { m_ammoInClip = ammo; }

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
            if (m_currentState != null)
            {
                m_currentState.Update(ref frameTime, triggerState, pos, facing);
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
        }

        public void ResetItem()
        {
            m_isDestroyed = false;
        }

        protected bool m_isDestroyed = true;
        protected Params m_params = null;

        private GameWorld m_world = null;
        private State m_currentState = null;
        private int m_ammoInClip = 0;


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
            public abstract void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing);
            public virtual void OnExit() {}

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

            public override void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing)
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
                Params param = GetWeapon().GetParams();
                GetWeapon().SetAmmoInClip(param.m_clipSize);
            }
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

            public override void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing)
            {
                // Empty is a dead-end state. Must be exited using a reload call
            }

            private State m_nextState = null;
        }


        #endregion

    }
}
