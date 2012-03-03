using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Debug;
using Momo.Core.Pathfinding;

using TestGame.Entities;
using Momo.Core.StateMachine;
using TestGame.Ai.AiEntityStates;



namespace TestGame.Ai.AiEntityStates
{
    class FindState : AiEntityState
    {
        #region Fields

        private static readonly int kUpdatePathFrameFrequency = 3;

        private PathRoute m_routeToPlayer = null;
        private PathTracker m_tracker = new PathTracker();

        #endregion

        #region Constructor

        public FindState(AiEntity entity) :
            base(entity)
        {
        }

        #endregion

        #region Public Interface

        public State FoundPlayerState { get; set; }

        public override string ToString()
        {
            return "Find";
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            // If we've found the player, go into the found player state
            if (AiEntity.SensoryData.StraightPathToPlayerSense != null)
            {
                AiEntity.StateMachine.CurrentState = FoundPlayerState;
            }
            else
            {
                PathNode myPathNode = null;
                PathNode goalPathNode = null;

                PathFindingHelpers.GetClosestPathNode( AiEntity.GetPosition(),
                                                       AiEntity.GetBin(),
                                                       BinLayers.kPathNodes,
                                                       AiEntity.ObstructionBinLayer,
                                                       ref myPathNode);

                PathFindingHelpers.GetClosestPathNode( AiEntity.World.PlayerManager.AveragePlayerPosition,
                                                       AiEntity.GetBin(),
                                                       BinLayers.kPathNodes,
                                                       AiEntity.ObstructionBinLayer,
                                                       ref goalPathNode);

                if (myPathNode != null && goalPathNode != null)
                {
                    bool cacheOnly = ((updateToken % kUpdatePathFrameFrequency) != 0);
                    AiEntity.World.PathRouteManager.GetPathRoute(myPathNode, goalPathNode, cacheOnly, ref m_routeToPlayer);
                }
                else
                {
                    m_routeToPlayer = null;
                }


                if (m_routeToPlayer != null)
                {
                    Vector2 targetDirection;
                    m_tracker.Track(    AiEntity.GetPosition(),
                                        AiEntity.World.PlayerManager.AveragePlayerPosition,
                                        AiEntity.GetBin(),
                                        AiEntity.ObstructionBinLayer,
                                        ref m_routeToPlayer,
                                        out targetDirection);


                    AiEntity.TurnTowards(targetDirection, 0.12f);
                    float speed = AiEntity.GetRelativeFacing(targetDirection) * 1.5f;
                    Vector2 newPosition = AiEntity.GetPosition() + AiEntity.FacingDirection * speed;

                    AiEntity.SetPosition(newPosition);
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

        #endregion
    }
}
