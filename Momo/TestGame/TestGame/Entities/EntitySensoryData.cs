using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using TestGame.Entities.Players;



namespace TestGame.Entities
{
    public enum SensedType
    {
        kNone = -1,
        kSeePlayer = 0,
        kSawPlayer,

        kStraightPathToPlayer,

        kNoise,
    }


    public class SensedObject
    {
        internal int m_id;
        internal SensedType m_type;

        internal Vector2 m_lastPosition;
        internal float m_sensedDistanceSq;
        internal float m_timeSensed;
        internal DynamicGameEntity m_sensedEntity;



        public SensedObject(int id, SensedType obj, Vector2 position, float distanceSq, float timeSensed, DynamicGameEntity entity)
        {
            m_id = id;
            m_type = obj;
            m_lastPosition = position;
            m_sensedDistanceSq = distanceSq;
            m_timeSensed = timeSensed;
            m_sensedEntity = entity;
        }


        public Vector2 GetLastPosition()
        {
            return m_lastPosition;
        }
    }


    public class EntitySensoryData
    {
        static readonly int kMaxSensedObjects = 10;


        private float m_invHalfSightFov = 0.0f;
        private float m_sightDistanceSq = 0.0f;
        private float m_senseDistanceSq = 0.0f;


        private SensedObject[] m_sensedObjects = new SensedObject[kMaxSensedObjects];
        private int m_sensedObjectCnt = 0;

        private SensedObject m_seePlayerSense = null;
        private SensedObject m_straightPathToPlayerSense = null;



        public SensedObject SeePlayerSense
        {
            get { return m_seePlayerSense; }
        }

        public SensedObject StraightPathToPlayerSense
        {
            get { return m_straightPathToPlayerSense; }
        }


        public EntitySensoryData(float sightFov, float sightDistance, float senseDistance)
        {
            m_invHalfSightFov = 1.0f - (float)Math.Sin(sightFov * 0.5f);
            m_sightDistanceSq = sightDistance * sightDistance;
            m_senseDistanceSq = senseDistance * senseDistance;

            Reset();
        }


        public void Reset()
        {
            for (int i = 0; i < m_sensedObjectCnt; ++i)
            {
                m_sensedObjects[i].m_type = SensedType.kNone;
                m_sensedObjects[i].m_sensedEntity = null;
            }

            m_sensedObjectCnt = 0;

            m_seePlayerSense = null;
            m_straightPathToPlayerSense = null;
        }


        public void Update(ref FrameTime frameTime)
        {
            m_seePlayerSense = null;
            m_straightPathToPlayerSense = null;


            for (int i = 0; i < m_sensedObjectCnt; ++i)
            {
                m_sensedObjects[i].m_timeSensed -= frameTime.Dt;

                if (m_sensedObjects[i].m_timeSensed < 0.0f)
                {
                    if (m_sensedObjects[i].m_type == SensedType.kSeePlayer)
                    {
                        AddSense(0, SensedType.kSawPlayer, m_sensedObjects[i].m_lastPosition, m_sensedObjects[i].m_sensedDistanceSq, 3.0f, m_sensedObjects[i].m_sensedEntity);
                    }

                    m_sensedObjects[i].m_type = SensedType.kNone;
                    m_sensedObjects[i].m_sensedEntity = null;

                    if (i < m_sensedObjectCnt - 2)
                    {
                        m_sensedObjects[i] = m_sensedObjects[m_sensedObjectCnt - 1];
                        --m_sensedObjectCnt;
                        --i;
                    }
                }
                else
                {
                    if (m_sensedObjects[i].m_type == SensedType.kSeePlayer)
                        m_seePlayerSense = m_sensedObjects[i];

                    if(m_sensedObjects[i].m_type == SensedType.kStraightPathToPlayer)
                        m_straightPathToPlayerSense = m_sensedObjects[i];
                }
            }
        }


        public void UpdateSensoryData(AiEntity aiEntity, PlayerEntity[] playerEntites)
        {
            for (int i = 0; i < playerEntites.Length; ++i)
            {
                if (playerEntites[i] != null)
                    UpdatePlayerSense(aiEntity, playerEntites[i]);
            }
        }


        public void AddSense(int id, SensedType obj, Vector2 position, float distanceSq, float timeSensed, DynamicGameEntity entity)
        {
            int objectIdx = GetSenseIndex(id, obj);

            // Existing sense does not exist
            if (objectIdx < 0)
            {
                if (m_sensedObjectCnt < kMaxSensedObjects)
                {
                    objectIdx = m_sensedObjectCnt;
                    ++m_sensedObjectCnt;
                }
                else
                {
                    objectIdx = GetSenseIndexWithLowestTime();
                }
            }


            m_sensedObjects[objectIdx] = new SensedObject(id, obj, position, distanceSq, timeSensed, entity);
        }


        public bool GetSensedObject(SensedType type, ref SensedObject obj)
        {
            int closestIndex = GetClosestSenseIndex(type);

            if (closestIndex >= 0)
            {
                obj = m_sensedObjects[closestIndex];
                return true;
            }

            obj = null;
            return false;
        }


        private int GetSenseIndex(int id, SensedType type)
        {
            for (int i = 0; i < m_sensedObjectCnt; ++i)
            {
                if (m_sensedObjects[i].m_id == id && m_sensedObjects[i].m_type == type)
                    return i;
            }

            return -1;
        }


        private int GetSenseIndex(SensedType type)
        {
            for (int i = 0; i < m_sensedObjectCnt; ++i)
            {
                if (m_sensedObjects[i].m_type == type)
                    return i;
            }

            return -1;
        }


        private int GetSenseIndexWithLowestTime()
        {
            float lowestTime = float.MaxValue;
            int lowestIndex = 0;

            for (int i = 0; i < m_sensedObjectCnt; ++i)
            {
                if (m_sensedObjects[i].m_timeSensed < lowestTime)
                    lowestIndex = i;
            }

            return lowestIndex;
        }


        private int GetClosestSenseIndex(SensedType type)
        {
            float closestDistSq = float.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < m_sensedObjectCnt; ++i)
            {
                if (m_sensedObjects[i].m_type == type && m_sensedObjects[i].m_sensedDistanceSq < closestDistSq)
                    closestIndex = i;
            }

            return closestIndex;
        }


        private void UpdatePlayerSense(AiEntity aiEntity, PlayerEntity playerEntity)
        {
            Vector2 dPos = playerEntity.GetPosition() - aiEntity.GetPosition();
            float distanceSq = dPos.LengthSquared();


            if (distanceSq < m_sightDistanceSq)
            {
                bool checkLineOfSight = distanceSq < m_senseDistanceSq;

                if (!checkLineOfSight)
                {
                    float distance = (float)Math.Sqrt(distanceSq);
                    Vector2 normlisedDPos = dPos / distance;
                    float angleToEntity = Vector2.Dot(normlisedDPos, aiEntity.FacingDirection);

                    checkLineOfSight = angleToEntity > m_invHalfSightFov;
                }

                if (checkLineOfSight)
                {
                    bool clearLineOfSight = CollisionHelpers.IsClearLineOfSight(aiEntity.GetPosition(), dPos, playerEntity.GetBin(), aiEntity.GetOccludingBinLayer());
                    bool clearLineOfPath = CollisionHelpers.IsClearLineOfSight(aiEntity.GetPosition(), dPos, playerEntity.GetBin(), aiEntity.GetObstructionBinLayer());

                    //if (clearLineOfSight)
                    //    entity.GetWorld().GetDebugRenderer().DrawFilledLine(myPosition, entity.GetPosition(), new Color(0.0f, 1.0f, 0.0f, 0.2f), 2.0f);
                    //else
                    //    entity.GetWorld().GetDebugRenderer().DrawFilledLine(myPosition, entity.GetPosition(), new Color(1.0f, 0.0f, 0.0f, 0.2f), 2.0f);


                    //int kBinSelectionCapacity = 1000;
                    //BinRegionSelection ms_tempBinRegionSelection = new BinRegionSelection(kBinSelectionCapacity);
                    //entity.GetBin().GetBinRegionFromLine(myPosition, dPos, ref ms_tempBinRegionSelection);

                    //if(clearLineOfSight)
                    //    entity.GetBin().DebugRender(entity.GetWorld().GetDebugRenderer(), ms_tempBinRegionSelection, new Color(0.0f, 1.0f, 0.0f, 0.2f));
                    //else
                    //    entity.GetBin().DebugRender(entity.GetWorld().GetDebugRenderer(), ms_tempBinRegionSelection, new Color(1.0f, 0.0f, 0.0f, 0.2f));



                    if (clearLineOfSight)
                        AddSense(0, SensedType.kSeePlayer, playerEntity.GetPosition(), distanceSq, 0.05f, playerEntity);

                    if (clearLineOfPath)
                        AddSense(0, SensedType.kStraightPathToPlayer, playerEntity.GetPosition(), distanceSq, 0.05f, playerEntity);
                }
            }
        }
    }
}
