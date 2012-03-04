using TestGame.Ai.AiEntityStates;

using Microsoft.Xna.Framework;

using Momo.Core.StateMachine;




namespace TestGame.Entities.AI
{
    public class Zombie : AiEntity
    {
        #region State Machine
        private WanderState mStateWander = null;
        #endregion


        public Zombie(GameWorld world)
            : base(world)
        {
            mBinLayer = BinLayers.kEnemyEntities;

            mStateWander = new WanderState(this);
            mStateWander.TimeInState = 10000.0f;

            SecondaryDebugColor = new Color(1.0f, 0.0f, 0.0f);
        }


        public override void ResetItem()
        {
            base.ResetItem();

            StateMachine.CurrentState = mStateWander;
        }

    }
}
