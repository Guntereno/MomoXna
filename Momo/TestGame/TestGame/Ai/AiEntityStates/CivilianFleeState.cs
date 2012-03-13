using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;

using Momo.Maths;

using TestGame.Entities;



namespace TestGame.Ai.AiEntityStates
{
    public class CivilianFleeState : TimedState
    {
        const float kStateBaseSpeedMod = 10.0f;

        //PathTracker mPathTracker;

        //float mLongFleeSearchAngle = 0.0f;
        //float mLongDirectionTimer = 0.0f;

        int mLongDirectionSearch = 1;
        int mLongDirectionIndex = 0;
        Vector2 mLongDirection = Vector2.Zero;



        public State IdleState { get; set; }


        struct SearchDirs
        {
            public float mX;
            public float mY;
        }

        static SearchDirs[] msSearchDirections = new SearchDirs[] {
            new SearchDirs(){ mX = 0.0f, mY = 0.75f },
            new SearchDirs(){ mX = 1.0f, mY = 0.75f },
            new SearchDirs(){ mX = 1.0f, mY = 0.0f },
            new SearchDirs(){ mX = 1.0f, mY = -0.75f },
            new SearchDirs(){ mX = 0.0f, mY = -0.75f }
        };



        public CivilianFleeState(AiEntity entity) :
            base(entity)
        {
            DebugColor = new Color(0.0f, 1.0f, 1.0f, 0.7f);
        }


        public override string ToString()
        {
            return "Flee";
        }


        public override void OnEnter()
        {
            base.OnEnter();

            AiEntity.Speed = AiEntity.BaseSpeed * kStateBaseSpeedMod;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            Zone world = AiEntity.Zone;
            Random random = world.Random;

            Vector2 boundaryRepelForce = Vector2.Zero;
            Vector2 entityRepelForce = Vector2.Zero;
            Vector2 longRepelForce = Vector2.Zero;

            float kEntityRepelRadius = AiEntity.ContactRadiusInfo.Radius * 30.0f;
            float kEntityRepelStr = 1.0f;
            //float kLongEntityRepelRadius = AiEntity.ContactRadiusInfo.Radius * 20.0f;
            float kLongPathRepelRadius = AiEntity.ContactRadiusInfo.Radius * 20.0f;
            float kLongRepelStr = 1.0f;
            float kBoundaryRepelRadius = AiEntity.ContactRadiusInfo.Radius * 2.0f;
            float kBoundaryRepelStr = 50.5f;

            //float kEntitySightRadius = 800.0f;
            //float kEntityHearRadius = 800.0f;
            //float kEntityViewDot = 0.5f;



            // Boundary repel
            if (updateToken % 2 == 0)
            {
                boundaryRepelForce = AiEntityStateHelpers.GetForceFromSurroundingBoundaries(kBoundaryRepelRadius, kBoundaryRepelRadius * kBoundaryRepelRadius, AiEntity, BinLayers.kBoundary);
                boundaryRepelForce *= kBoundaryRepelStr;
            }
            // Entity repel
            if ((updateToken + 1) % 5 == 0)
            {
                entityRepelForce = -AiEntityStateHelpers.GetForceFromSurroundingEntities(kEntityRepelRadius, kEntityRepelRadius * kEntityRepelRadius, AiEntity, BinLayers.kEnemyEntities);
                entityRepelForce *= kEntityRepelStr;
            }

            //mLongDirectionTimer += frameTime.Dt;


            if ((updateToken + 2) % 10 == 0)
            {
                Vector2 perpDirection = Maths2D.Perpendicular(AiEntity.FacingDirection);

                Vector2 searchDirection = Vector2.Zero;
                bool clearPath = false;

                mLongDirection = Vector2.Zero;


                SearchDirs searchDir = msSearchDirections[mLongDirectionIndex];
                searchDirection = ((AiEntity.FacingDirection * searchDir.mX) + (perpDirection * searchDir.mY));
                clearPath = AiEntityStateHelpers.IsClearLineOfSightBoundary(AiEntity.GetPosition(), searchDirection * kLongPathRepelRadius, AiEntity.Zone.Bin, BinLayers.kBoundaryObstructionSmall);
                int enemyCnt = AiEntityStateHelpers.CountBinItems(AiEntity.GetPosition(), searchDirection * kLongPathRepelRadius, AiEntity.Zone.Bin, BinLayers.kEnemyEntities);


                mLongDirectionIndex += mLongDirectionSearch;

                if (mLongDirectionIndex == msSearchDirections.Length - 1 ||
                    mLongDirectionIndex == 0)
                {
                    mLongDirectionSearch = -mLongDirectionSearch;
                }

                if (clearPath && enemyCnt == 0)
                {
                    mLongDirection = searchDirection;
                    mLongDirectionIndex = 2;
                }
            }


            longRepelForce = mLongDirection * kLongRepelStr;


            Vector2 walkDirection = entityRepelForce;
            walkDirection += boundaryRepelForce;
            walkDirection += longRepelForce;
            walkDirection += AiEntity.FacingDirection * 0.5f;

            float walkDirectionLenSqrd = walkDirection.LengthSquared();
            if (walkDirectionLenSqrd > 0.0f)
            {
                float walkDirectionLen = (float)System.Math.Sqrt(walkDirectionLenSqrd);
                Vector2 walkDirectionNorm = walkDirection / walkDirectionLen;
                AiEntity.TurnTowardsAndWalk(walkDirectionNorm, 0.10f, AiEntity.Speed * frameTime.Dt);
            }


            //if (updateToken % 90 == 0)
            //{
            //    GameEntity closestEntity = null;
            //    Vector2 closetDPosition = Vector2.Zero;
            //    if (!AiEntityStateHelpers.GetEntities(kEntitySightRadius, kEntityHearRadius, kEntityViewDot, AiEntity, BinLayers.kEnemyList, BinLayers.kBoundaryOcclusionSmall, ref closestEntity, ref closetDPosition))
            //    {
            //        AiEntity.StateMachine.CurrentState = IdleState;
            //    }
            //}
        }
    }
}
