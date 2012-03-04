using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Momo.Core.ObjectPools;
using Microsoft.Xna.Framework;
using Momo.Core.StateMachine;

namespace TestGame.Weapons
{
    public abstract class Weapon : IPoolItem, IStateMachineOwner
    {
        #region Constructor

        public Weapon(GameWorld world)
        {
            m_world = world;

            StateMachine = new StateMachine(this);
        }

        #endregion

        #region Fields

        protected bool m_isDestroyed = true;
        protected GunParams m_params = null;

        private GameWorld m_world = null;
        private int m_ammoInClip = 0;

        private Vector2 m_position = Vector2.Zero;
        private Vector2 m_barrelPos = Vector2.Zero;
        private Vector2 m_direction = Vector2.Zero;
        private Vector2 m_recoil = Vector2.Zero;
        private float m_facing = 0.0f;

        private float m_triggerState = 0.0f;

        private IWeaponUser m_owner = null;

        #endregion


        #region Public Interface

        public StateMachine StateMachine { get; private set; }

        public Vector2 Recoil
        {
            get { return m_recoil; }
            set { m_recoil = value; }
        }

        public IWeaponUser Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public int AmmoInClip
        {
            get { return m_ammoInClip; }
            set { m_ammoInClip = value; }
        }

        public GameWorld World { get { return m_world; } }
        public GunParams Parameters { get { return m_params; } }
        public Vector2 Position { get { return m_position; } }
        public Vector2 BarrelPosition { get { return m_barrelPos; } }
        public Vector2 Direction { get { return m_direction; } }
        public float Facing { get { return m_facing; } }
        public float TriggerState { get { return m_triggerState; } }

        public abstract bool AcceptingInput { get; }

        public override string ToString()
        {
            return "";
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

            StateMachine.Update(ref frameTime, 0);
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
            World.WeaponManager.TriggerCoalesce();
        }

        public void ResetItem()
        {
            m_isDestroyed = false;
        }

        #endregion

        #region Params

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

        #endregion

        #region States
        public abstract class WeaponState: State
        {
            #region Constructor

            public WeaponState(Weapon weapon): base(weapon)
            {
            }

            #endregion

            #region Public Interface

            public override string ToString()
            {
                return GetType().Name;
            }

            public Weapon Weapon
            {
                get { return Owner as Weapon; }
            }

            #endregion
        }

        public abstract class TimedState : WeaponState
        {
            #region Fields

            float m_timer;

            #endregion

            #region Constructor

            public TimedState(Weapon weapon) :
                base(weapon)
            { }

            #endregion

            #region Public Interface

            public State NextState { get; set; }
            public float Length { get; set; }

            public override string ToString()
            {
                return GetType().Name + " (" + m_timer.ToString("F3") + ")";
            }

            public override void OnEnter()
            {
                base.OnEnter();

                m_timer = 0.0f;
            }

            public override void Update(ref FrameTime frameTime, int updateToken)
            {
                m_timer += frameTime.Dt;
                if (m_timer >= Length)
                {
                    Weapon.StateMachine.CurrentState = NextState;
                }
            }

            #endregion
        }

        public class ReloadState : TimedState
        {
            #region Constructor

            public ReloadState(Weapon weapon) :
                base(weapon)
            {}

            #endregion


            #region Public Interface

            public override void OnEnter()
            {
                Length = Weapon.Parameters.m_reloadTime;

                base.OnEnter();
            }

            public override void OnExit()
            {
                GunParams param = Weapon.Parameters;
                Weapon.AmmoInClip = param.m_clipSize;
            }

            #endregion
        }


        public class CoolDownState : TimedState
        {
            #region Constructor

            public CoolDownState(Weapon weapon) :
                base(weapon)
            {}

            #endregion

            #region Public Interface

            public override void OnEnter()
            {
                Length = 1.0f / Weapon.Parameters.m_fireRate;

                base.OnEnter();
            }

            #endregion
        }

        public class EmptyState : WeaponState
        {
            #region Constructor

            public EmptyState(Weapon weapon) :
                base(weapon)
            { }

            #endregion

            #region Public Interface

            public State NextState { get; set; }

            public override void Update(ref FrameTime frameTime, int updateToken)
            {
                if (Weapon.AmmoInClip > 0)
                {
                    Weapon.StateMachine.CurrentState = NextState;
                }
            }

            #endregion
        }

        #endregion

    }
}
