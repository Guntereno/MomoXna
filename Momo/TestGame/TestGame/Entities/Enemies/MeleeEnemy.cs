﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Ai.States;

namespace TestGame.Entities.Enemies
{
    public class MeleeEnemy : AiEntity
    {
        private RandomWanderState m_stateRandomWander = null;
        private FindState m_stateFind = null;
        private ChaseState m_stateChase = null;

        public MeleeEnemy(GameWorld world)
            : base(world)
        {
            m_stateRandomWander = new RandomWanderState(this);
            m_stateFind = new FindState(this);
            m_stateChase = new ChaseState(this);
            m_stateStunned = new StunnedState(this);
            m_stateDying = new DyingState(this);

            m_stateRandomWander.Init(m_stateChase);
            m_stateFind.Init(m_stateChase);
            m_stateChase.Init(m_stateFind);
            m_stateStunned.Init(m_stateChase);
        }

        public void Init()
        {
            SetCurrentState(m_stateFind);
        }

    }
}