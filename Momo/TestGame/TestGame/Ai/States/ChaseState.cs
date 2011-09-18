using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Pathfinding;
using Momo.Debug;

using TestGame.Entities;




namespace TestGame.Ai.States
{
    public class ChaseState : State
    {
        private static readonly int kUpdatePathFrameFrequency = 3;

        private PathRoute m_routeToPlayer = null;
        private PathTracker m_tracker = new PathTracker();

        private State m_lostPlayerState = null;



        public ChaseState(AiEntity entity) :
            base(entity)
        {
        }


        public override string ToString()
        {
            return "Chase";
        }


        public void Init(State lostPlayerState)
        {
            m_lostPlayerState = lostPlayerState;
        }


        public override void OnEnter()
        {
            base.OnEnter();

            GetEntity().DebugColor = new Color(1.0f, 0.5f, 0.0f, 0.5f);
        }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            AiEntity entity = GetEntity();


            if (entity.SensoryData.SensePlayer)
            {
                SensedObject obj = null;

                if (entity.SensoryData.GetClosestSense(SensedType.kSeePlayer, ref obj))
                {
                    Vector2 targetDirection = obj.GetLastPosition() - GetEntity().GetPosition();
                    targetDirection.Normalize();

                    GetEntity().TurnTowards(targetDirection, 0.11f);
                    Vector2 newPosition = GetEntity().GetPosition() + GetEntity().FacingDirection * 2.0f;

                    GetEntity().SetPosition(newPosition);
                }
            }
            else
            {
                PathNode myPathNode = null;
                PathNode goalPathNode = null;

                PathFindingHelpers.GetClosestPathNode(entity.GetPosition(), entity.GetBin(), BinLayers.kPathNodes, entity.GetOccludingBinLayer(), ref myPathNode);
                PathFindingHelpers.GetClosestPathNode(entity.GetWorld().GetPlayerManager().GetAveragePosition(), entity.GetBin(), BinLayers.kPathNodes, entity.GetOccludingBinLayer(), ref goalPathNode);

                if (myPathNode != null && goalPathNode != null)
                {
                    bool cacheOnly = ((updateToken % kUpdatePathFrameFrequency) != 0);
                    entity.GetWorld().GetPathRouteManager().GetPathRoute(myPathNode, goalPathNode, cacheOnly, ref m_routeToPlayer);
                }
                else
                {
                    m_routeToPlayer = null;
                }


                if (m_routeToPlayer != null)
                {
                    Vector2 targetDirection;
                    m_tracker.Track(entity.GetPosition(),
                                        entity.GetWorld().GetPlayerManager().GetAveragePosition(),
                                        entity.GetBin(),
                                        entity.GetOccludingBinLayer(),
                                        ref m_routeToPlayer,
                                        out targetDirection);


                    GetEntity().TurnTowards(targetDirection, 0.11f);

                    Vector2 newPosition = GetEntity().GetPosition() + entity.FacingDirection * 1.5f;

                    GetEntity().SetPosition(newPosition);
                }

                //entity.SetCurrentState(m_lostPlayerState);
            }



        }


        public override void OnExit()
        {
            base.OnExit();

            m_routeToPlayer = null;
            m_tracker.Reset();
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            base.DebugRender(debugRenderer);

            //if (m_routeToPlayer != null)
            //    m_routeToPlayer.DebugRender(debugRenderer);

            //m_tracker.DebugRender(debugRenderer);
        }
    }
}
