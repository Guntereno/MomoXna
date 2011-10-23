using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Debug;
using Momo.Core.Pathfinding;
using Momo.Core.StateMachine;

using TestGame.Entities;


//namespace TestGame.Entities.Imps.States
//{
    //class FindState : ImpState
    //{
        //private static readonly int kUpdatePathFrameFrequency = 3;

        //private PathRoute m_routeToPlayer = null;
        //private PathTracker m_tracker = new PathTracker();

        //private State m_foundPlayerState = null;


        //public FindState(Imp owner, StateMachine stateMachine) :
        //    base(owner, stateMachine)
        //{

        //}

        //public void Init(State foundPlayerState)
        //{
        //    m_foundPlayerState = foundPlayerState;
        //}

        //public override string ToString()
        //{
        //    return "Find";
        //}

        //public override void OnEnter()
        //{
        //    base.OnEnter();

        //    m_owner.PrimaryDebugColor = new Color(1.0f, 0.72f, 0.0f, 0.5f);
        //}

        //public override void Update(ref FrameTime frameTime, int updateToken)
        //{
        //    // If we've found the player, go into the found player state
        //    //if (m_owner.SensoryData.StraightPathToPlayerSense != null)
        //    //{
        //    //    m_owner.CurrentState = m_foundPlayerState;
        //    //}
        //    //else
        //    {
        //        PathNode myPathNode = null;
        //        PathNode goalPathNode = null;

        //        PathFindingHelpers.GetClosestPathNode(m_owner.GetPosition(), m_owner.GetBin(), BinLayers.kPathNodes, m_owner.GetObstructionBinLayer(), ref myPathNode);
        //        PathFindingHelpers.GetClosestPathNode(m_owner.World.PlayerManager.GetAveragePosition(), m_owner.GetBin(), BinLayers.kPathNodes, m_owner.GetObstructionBinLayer(), ref goalPathNode);

        //        if (myPathNode != null && goalPathNode != null)
        //        {
        //            bool cacheOnly = ((updateToken % kUpdatePathFrameFrequency) != 0);
        //            m_owner.World.PathRouteManager.GetPathRoute(myPathNode, goalPathNode, cacheOnly, ref m_routeToPlayer);
        //        }
        //        else
        //        {
        //            m_routeToPlayer = null;
        //        }


        //        if (m_routeToPlayer != null)
        //        {
        //            Vector2 targetDirection;
        //            m_tracker.Track(m_owner.GetPosition(),
        //                                m_owner.World.PlayerManager.GetAveragePosition(),
        //                                m_owner.GetBin(),
        //                                m_owner.GetObstructionBinLayer(),
        //                                ref m_routeToPlayer,
        //                                out targetDirection);


        //            m_owner.TurnTowards(targetDirection, 0.12f);
        //            float speed = m_owner.GetRelativeFacing(targetDirection) * 1.5f;
        //            Vector2 newPosition = m_owner.GetPosition() + m_owner.FacingDirection * speed;

        //            m_owner.SetPosition(newPosition);
        //        }
        //    }
        //}

        //public override void OnExit()
        //{
        //    base.OnExit();

        //    m_routeToPlayer = null;
        //    m_tracker.Reset();
        //}

        //public override void DebugRender(DebugRenderer debugRenderer)
        //{
        //    base.DebugRender(debugRenderer);

        //    //if (m_routeToPlayer != null)
        //    //    m_routeToPlayer.DebugRender(debugRenderer);

        //    //m_tracker.DebugRender(debugRenderer);
        //}

    //}
//}
