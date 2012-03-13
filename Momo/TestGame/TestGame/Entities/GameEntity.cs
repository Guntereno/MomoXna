using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.ObjectPools;
using Momo.Core.Collision2D;
using Momo.Debug;
using Momo.Maths;

using TestGame.Entities.Gaits;



namespace TestGame.Entities
{
    public class GameEntity : DynamicEntity, INamed, IPoolItem
    {
        // --------------------------------------------------------------------
        // -- Variables
        // --------------------------------------------------------------------
        private const int kMaxNameLength = 32;

        private Name mName = new Name(kMaxNameLength);

        private RadiusInfo mContactRadiusInfo;
        private CollidableGroupInfo mCollidableGroupInfo = new CollidableGroupInfo();

        private float mFacingAngle = 0.0f;
        private Vector2 mFacingDirection = Vector2.Zero;

        private Gait mGait = new Gait();

        private float mBaseSpeed = 1.0f;
        private float mSpeed = 1.0f;

        private Color mPrimaryDebugColour = Color.White;
        private Color mSecondaryDebugColour = Color.White;

        private bool mDestroyed = false;

        private Zone mZone;


        protected int mBinLayer = 0;


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties
        public Zone Zone { get { return mZone; } }

        public float FacingAngle
        {
            get { return mFacingAngle; }
            set
            {
                System.Diagnostics.Debug.Assert(!float.IsNaN(value), "Gotcha you little git. This occasionally happens seemly for no reason. We should be able to work backwards from here.");
                mFacingAngle = value;
                mFacingDirection = new Vector2((float)Math.Sin(mFacingAngle), (float)Math.Cos(mFacingAngle));
            }
        }

        public Vector2 FacingDirection
        {
            get { return mFacingDirection; }
            set
            {
                System.Diagnostics.Debug.Assert(!float.IsNaN(value.X), "Gotcha you little git. This occasionally happens seemly for no reason. We should be able to work backwards from here.");
                mFacingDirection = value;
                mFacingAngle = (float)Math.Atan2(mFacingDirection.X, mFacingDirection.Y);
            }
        }

        public Gait Gait
        {
            get { return mGait; }
            set { mGait = value; }
        }

        public float BaseSpeed
        {
            get { return mBaseSpeed; }
            set { mBaseSpeed = value; }
        }

        public float Speed
        {
            get { return mSpeed; }
            set { mSpeed = value; }
        }

        public Color PrimaryDebugColor
        {
            get { return mPrimaryDebugColour; }
            set { mPrimaryDebugColour = value; }
        }

        public Color SecondaryDebugColor
        {
            get { return mSecondaryDebugColour; }
            set { mSecondaryDebugColour = value; }
        }

        public RadiusInfo ContactRadiusInfo
        {
            get { return mContactRadiusInfo; }
            set { mContactRadiusInfo = value; }
        }

        public CollidableGroupInfo CollidableGroupInfo
        {
            get { return mCollidableGroupInfo; }
            set { mCollidableGroupInfo = value; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Methods
        // --------------------------------------------------------------------
        public GameEntity(Zone zone)
        {
            mZone = zone;
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawCircle(GetPosition(), ContactRadiusInfo.Radius, mPrimaryDebugColour, mSecondaryDebugColour, true, 2.5f, 8);
            debugRenderer.DrawLine(GetPosition(), GetPosition() + (mFacingDirection * mContactRadiusInfo.Radius * 1.5f), mSecondaryDebugColour);
        }


        public float GetRelativeFacing(Vector2 direction)
        {
            float dotFacingDirecion = Vector2.Dot(mFacingDirection, direction);
            return 0.5f + (dotFacingDirecion * 0.5f);
        }

        public void TurnTowardsAndWalk(Vector2 targetDirection, float turnSpeed, float amount)
        {
            TurnTowards(targetDirection, turnSpeed);

            float relativeDirection = GetRelativeFacing(targetDirection);

            mGait.WalkForward(this, relativeDirection * amount);
            //SetPosition(GetPosition() + (FacingDirection * (relativeDirection * walkSpeed)));
        }

        // Returns [0.0, 1.0] based on how close the facing is to the target. 1.0 = the same, 0.0 opposite.
        public void TurnTowards(Vector2 targetDirection, float turnSpeed)
        {
            Vector2 normalToFacing = Maths2D.Perpendicular(mFacingDirection);
            float dotNormalTargetDirection = Vector2.Dot(normalToFacing, targetDirection);

            Vector2 newFacing = mFacingDirection;

            if (dotNormalTargetDirection > 0.0f)
            {
                newFacing += (normalToFacing * turnSpeed);

                if (Vector2.Dot(Maths2D.Perpendicular(newFacing), targetDirection) < 0.0f)
                    newFacing = targetDirection;
            }
            else
            {
                newFacing -= (normalToFacing * turnSpeed);

                if (Vector2.Dot(Maths2D.Perpendicular(newFacing), targetDirection) > 0.0f)
                    newFacing = targetDirection;
            }

            newFacing.Normalize();

            FacingDirection = newFacing;
        }

        public virtual void OnCollisionEvent(ref BulletEntity bullet)
        {
        }


        // --------------------------------------------------------------------
        // -- IPool
        // --------------------------------------------------------------------
        public bool IsDestroyed()
        {
            return mDestroyed;
        }

        public void DestroyItem()
        {
            mZone.Bin.RemoveBinItem(this, mBinLayer);
            mDestroyed = true;
        }

        public virtual void ResetItem()
        {
            mDestroyed = false;
        }


        // --------------------------------------------------------------------
        // -- INamed
        // --------------------------------------------------------------------
        public void SetName(MutableString name)
        {
            mName.SetName(name);
        }

        public void SetName(string name)
        {
            mName.SetName(name);
        }

        public MutableString GetName()
        {
            return mName.GetName();
        }

        public int GetNameHash()
        {
            return mName.GetNameHash();
        }
    }
}
