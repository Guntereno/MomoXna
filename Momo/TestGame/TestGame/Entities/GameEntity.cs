using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.ObjectPools;
using Momo.Core.Collision2D;
using Momo.Debug;
using Momo.Maths;



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

        private float mBaseSpeed = 1.0f;

        private Color mPrimaryDebugColour = Color.White;
        private Color mSecondaryDebugColour = Color.White;

        private bool mDestroyed = false;

        private GameWorld mWorld;


        protected int mBinLayer = 0;


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties
        public GameWorld World { get { return mWorld; } }

        public float FacingAngle
        {
            get { return mFacingAngle; }
            set
            {
                mFacingAngle = value;
                mFacingDirection = new Vector2((float)Math.Sin(mFacingAngle), (float)Math.Cos(mFacingAngle));
            }
        }

        public Vector2 FacingDirection
        {
            get { return mFacingDirection; }
            set
            {
                mFacingDirection = value;
                mFacingAngle = (float)Math.Atan2(mFacingDirection.X, mFacingDirection.Y);
            }
        }

        public float BaseSpeed
        {
            get { return mBaseSpeed; }
            set { mBaseSpeed = value; }
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
        public GameEntity(GameWorld world)
        {
            mWorld = world;
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            Color fillColour = mPrimaryDebugColour;
            fillColour.A = 102;
            Color outlineColour = mSecondaryDebugColour;
            outlineColour.A = 86;

            debugRenderer.DrawCircle(GetPosition(), ContactRadiusInfo.Radius, fillColour, outlineColour, true, 3.5f, 8);
            debugRenderer.DrawLine(GetPosition(), GetPosition() + (mFacingDirection * mContactRadiusInfo.Radius * 1.5f), outlineColour);
        }


        public float GetRelativeFacing(Vector2 direction)
        {
            float dotFacingDirecion = Vector2.Dot(mFacingDirection, direction);
            return 0.5f + (dotFacingDirecion * 0.5f);
        }

        public void TurnTowardsAndWalk(Vector2 targetDirection, float speed)
        {
            TurnTowards(targetDirection, speed);

            float relativeDirection = GetRelativeFacing(targetDirection);

            SetPosition(GetPosition() + (FacingDirection * relativeDirection));
        }

        // Returns [0.0, 1.0] based on how close the facing is to the target. 1.0 = the same, 0.0 opposite.
        public void TurnTowards(Vector2 targetDirection, float speed)
        {
            Vector2 normalToFacing = Maths2D.Perpendicular(mFacingDirection);
            float dotNormalTargetDirection = Vector2.Dot(normalToFacing, targetDirection);

            Vector2 newFacing = mFacingDirection;

            if (dotNormalTargetDirection > 0.0f)
            {
                newFacing += (normalToFacing * speed);

                if (Vector2.Dot(Maths2D.Perpendicular(newFacing), targetDirection) < 0.0f)
                    newFacing = targetDirection;
            }
            else
            {
                newFacing -= (normalToFacing * speed);

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
