using Momo.Core.Pathfinding;
using TestGame.Entities;
using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Debug;

namespace TestGame.Ai.States
{
    class FindState : State
    {
        private static readonly int kUpdatePathFrameFrequency = 3;

        private PathRoute m_routeToPlayer = null;
        private PathTracker m_tracker = new PathTracker();

        private State m_foundPlayerState = null;


        public FindState(AiEntity entity) :
            base(entity)
        {
        }

        public void Init(State foundPlayerState)
        {
            m_foundPlayerState = foundPlayerState;
        }

        public override string ToString()
        {
            return "Find";
        }

        public override void OnEnter()
        {
            base.OnEnter();

            GetEntity().DebugColor = new Color(1.0f, 0.72f, 0.0f, 0.5f);
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            AiEntity entity = GetEntity();

            // If we've found the player, go into the found player state
            if (entity.SensoryData.StraightPathToPlayerSense != null)
            {
                entity.SetCurrentState(m_foundPlayerState);
            }
            else
            {
                PathNode myPathNode = null;
                PathNode goalPathNode = null;

                PathFindingHelpers.GetClosestPathNode(entity.GetPosition(), entity.GetBin(), BinLayers.kPathNodes, entity.GetObstructionBinLayer(), ref myPathNode);
                PathFindingHelpers.GetClosestPathNode(entity.GetWorld().PlayerManager.GetAveragePosition(), entity.GetBin(), BinLayers.kPathNodes, entity.GetObstructionBinLayer(), ref goalPathNode);

                if (myPathNode != null && goalPathNode != null)
                {
                    bool cacheOnly = ((updateToken % kUpdatePathFrameFrequency) != 0);
                    entity.GetWorld().PathRouteManager.GetPathRoute(myPathNode, goalPathNode, cacheOnly, ref m_routeToPlayer);
                }
                else
                {
                    m_routeToPlayer = null;
                }


                if (m_routeToPlayer != null)
                {
                    Vector2 targetDirection;
                    m_tracker.Track(    entity.GetPosition(),
                                        entity.GetWorld().PlayerManager.GetAveragePosition(),
                                        entity.GetBin(),
                                        entity.GetObstructionBinLayer(),
                                        ref m_routeToPlayer,
                                        out targetDirection);


                    entity.TurnTowards(targetDirection, 0.12f);
                    float speed = entity.GetRelativeFacing(targetDirection) * 1.5f;
                    Vector2 newPosition = GetEntity().GetPosition() + entity.FacingDirection * speed;

                    GetEntity().SetPosition(newPosition);
                }
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
