using TestGame.Ai.AiEntityStates;
using TestGame.Entities.Gaits;

using Microsoft.Xna.Framework;

using Momo.Core.StateMachine;




namespace TestGame.Entities.AI
{
    public class Zombie : AiEntity
    {
        #region State Machine
        private ZombieHerdState mStateHerd = null;
        private ZombieWanderState mStateWander = null;
        private ZombieChaseState mStateChase = null;
        #endregion


        public Zombie(Zone zone)
            : base(zone)
        {
            mBinLayer = BinLayers.kEnemyEntities;

            mStateWander = new ZombieWanderState(this);
            mStateWander.MinimumTimeInState = 1.0f;
            mStateWander.MaximumTimeInState = 5.0f;

            mStateHerd = new ZombieHerdState(this);
            mStateHerd.MinimumTimeInState = 8.0f;
            mStateHerd.MaximumTimeInState = 20.0f;

            mStateChase = new ZombieChaseState(this);
            mStateChase.TimeInState = float.MaxValue;

            mStateWander.HerdState = mStateHerd;
            mStateWander.ChaseState = mStateChase;
            mStateWander.NextState = mStateHerd;

            mStateHerd.ChaseState = mStateChase;
            mStateHerd.NextState = mStateWander;

            mStateChase.NextState = mStateWander;

            Gait = new ZombieGait((float)Zone.Random.NextDouble() * 100.0f);

            BaseSpeed = 10.0f + ((float)Zone.Random.NextDouble() * 5.0f);

            PrimaryDebugColor = new Color(1.0f, 0.0f, 0.0f, 0.3f);
            SecondaryDebugColor = PrimaryDebugColor;
        }


        public override void ResetItem()
        {
            base.ResetItem();

            StateMachine.CurrentState = mStateHerd;
        }

    }
}
