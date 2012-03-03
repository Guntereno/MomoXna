using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Pathfinding;
using Momo.Debug;

using TestGame.Entities;
using Momo.Core.StateMachine;
using TestGame.Ai.AiEntityStates;




namespace TestGame.Ai.AiEntityStates
{
    public class ChaseState : AiEntityState
    {
        #region Constructor

        public ChaseState(AiEntity entity) :
            base(entity)
        {
        }

        #endregion

        #region Public Interface

        public State LostPlayerState { get; set; }

        public override string ToString()
        {
            return "Chase";
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            SensedObject obj = AiEntity.SensoryData.StraightPathToPlayerSense;

            if (obj != null)
            {
                Vector2 targetDirection = obj.GetLastPosition() - AiEntity.GetPosition();
                targetDirection.Normalize();

                AiEntity.TurnTowards(targetDirection, 0.12f);
                float speed = AiEntity.GetRelativeFacing(targetDirection) * 1.8f;
                Vector2 newPosition = AiEntity.GetPosition() + AiEntity.FacingDirection * speed;

                AiEntity.SetPosition(newPosition);
            }
            else
            {
                AiEntity.StateMachine.CurrentState = LostPlayerState;
            }
        }


        public override void OnExit()
        {
            base.OnExit();
        }

        #endregion
    }
}
