﻿using Game.Ai.AiEntityStates;
using Game.Entities.Gaits;

using Microsoft.Xna.Framework;

using Momo.Core.StateMachine;




namespace Game.Entities.AI
{
    public class Zombie : AiEntity
    {
        #region State Machine
        private ZombieWanderState mStateWander = null;
        private ZombieHerdState mStateHerd = null;
        private ZombieChaseState mStateChase = null;
        private ZombieAttackState mStateAttack = null;
        #endregion


        public Zombie(Zone zone)
            : base(zone)
        {
            mBinLayer = BinLayers.kEnemyEntities;

            mStateWander = new ZombieWanderState(this);
            mStateWander.MinimumTimeInState = 1.0f;
            mStateWander.MaximumTimeInState = 3.0f;

            mStateHerd = new ZombieHerdState(this);
            mStateHerd.MinimumTimeInState = 6.0f;
            mStateHerd.MaximumTimeInState = 10.0f;

            mStateChase = new ZombieChaseState(this);
            mStateChase.TimeInState = float.MaxValue;

            mStateAttack = new ZombieAttackState(this);
            mStateAttack.TimeInState = float.MaxValue;

            mStateWander.HerdState = mStateHerd;
            mStateWander.ChaseState = mStateChase;
            mStateWander.NextState = mStateHerd;

            mStateHerd.ChaseState = mStateChase;
            mStateHerd.WanderState = mStateWander;
            mStateHerd.NextState = mStateWander;

            mStateChase.WanderState = mStateWander;
            mStateChase.AttackState = mStateAttack;

            mStateAttack.ChaseState = mStateChase;


            ZombieGait gait = new ZombieGait((float)Zone.Random.NextDouble() * 100.0f);
            gait.SwayLeft = (float)Zone.Random.NextDouble() * 0.185f;
            gait.SwayRight = (float)Zone.Random.NextDouble() * 0.185f;
            Gait = gait;
 
            BaseSpeed = 9.5f + ((float)Zone.Random.NextDouble() * 3.0f);

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
