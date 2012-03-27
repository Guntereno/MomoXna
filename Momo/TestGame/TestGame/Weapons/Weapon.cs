using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Momo.Core.ObjectPools;
using Microsoft.Xna.Framework;
using Momo.Core.StateMachine;

namespace Game.Weapons
{
    public abstract class Weapon : IPoolItem, IStateMachineOwner
    {
        #region Constructor

        public Weapon(Zone zone)
        {
            mZone = zone;

            StateMachine = new StateMachine(this);
        }

        #endregion

        #region Fields

        protected bool mIsDestroyed = true;
        protected GunParams mParams = null;

        private Zone mZone = null;
        private int mAmmoInClip = 0;

        private Vector2 mPosition = Vector2.Zero;
        private Vector2 mBarrelPos = Vector2.Zero;
        private Vector2 mDirection = Vector2.Zero;
        private Vector2 mRecoil = Vector2.Zero;
        private float mFacing = 0.0f;

        private float mTriggerState = 0.0f;

        private IWeaponUser mOwner = null;

        #endregion


        #region Public Interface

        public StateMachine StateMachine { get; private set; }

        public Vector2 Recoil
        {
            get { return mRecoil; }
            set { mRecoil = value; }
        }

        public IWeaponUser Owner
        {
            get { return mOwner; }
            set { mOwner = value; }
        }

        public int AmmoInClip
        {
            get { return mAmmoInClip; }
            set { mAmmoInClip = value; }
        }

        public Zone Zone { get { return mZone; } }
        public GunParams Parameters { get { return mParams; } }
        public Vector2 Position { get { return mPosition; } }
        public Vector2 BarrelPosition { get { return mBarrelPos; } }
        public Vector2 Direction { get { return mDirection; } }
        public float Facing { get { return mFacing; } }
        public float TriggerState { get { return mTriggerState; } }

        public abstract bool AcceptingInput { get; }

        public override string ToString()
        {
            return "";
        }

        public virtual void Init()
        {
            mAmmoInClip = mParams.m_clipSize;
        }

        public void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing)
        {
            Recoil = Vector2.Zero;

            mPosition = pos;
            mFacing = facing;
            mDirection = new Vector2((float)Math.Sin(facing), (float)Math.Cos(facing));

            mTriggerState = triggerState;

            // This should probably be a weapon parameter
            const float kWeaponLength = 5.0f;
            mBarrelPos = pos + (mDirection * kWeaponLength);

            StateMachine.Update(ref frameTime, 0);
        }

        public virtual void Reload()
        {
            mAmmoInClip = mParams.m_clipSize;
        }

        public bool IsDestroyed()
        {
            return mIsDestroyed;
        }

        public void DestroyItem()
        {
            mIsDestroyed = true;
        }

        public void ResetItem()
        {
            mIsDestroyed = false;
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

            float mTimer;

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
                return GetType().Name + " (" + mTimer.ToString("F3") + ")";
            }

            public override void OnEnter()
            {
                base.OnEnter();

                mTimer = 0.0f;
            }

            public override void Update(ref FrameTime frameTime, uint updateToken)
            {
                mTimer += frameTime.Dt;
                if (mTimer >= Length)
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

            public override void Update(ref FrameTime frameTime, uint updateToken)
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
