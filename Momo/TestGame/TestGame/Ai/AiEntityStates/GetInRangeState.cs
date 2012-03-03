using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Core.StateMachine;
using TestGame.Entities;

namespace TestGame.Ai.AiEntityStates
{
    class GetInRangeState : ChargeState
    {
        #region Constructor

        public GetInRangeState(AiEntity entity) :
            base(entity)
        {
        }

        #endregion

        #region Public Interface

        public State InRangeState { get; set; }
        public RadiusInfo Range { get; set; }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            // Follow the normal chase state
            base.Update(ref frameTime, updateToken);

            // Until the range is reached
            SensedObject sensedPlayer = AiEntity.SensoryData.SeePlayerSense;

            if (sensedPlayer != null)
            {
                Vector2 offset = sensedPlayer.GetLastPosition() - AiEntity.GetPosition();
                if (offset.LengthSquared() <= Range.RadiusSq)
                {
                    AiEntity.StateMachine.CurrentState = InRangeState;
                }
            }
        }

        #endregion
    }
}
