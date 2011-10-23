using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.StateMachine;
using Momo.Core.Spatial;

using TestGame.Objects;
using TestGame.Weapons;



namespace TestGame.Entities.Imps
{
    public class Imp : AiEntity
    {
        #region State Machine
        private StateMachine m_stateMachine = null;
        //private ActiveState m_stateActive = null;
        //private DeadState m_stateDead = null;
        //private DyingState m_stateDying = null;
        #endregion



        public Imp(GameWorld world)
            : base(world)
        {
            ContactRadiusInfo = new RadiusInfo(16.0f);
            SetMass(ContactRadiusInfo.Radius * 2.0f);

            PrimaryDebugColor = new Color(0.0f, 1.0f, 1.0f);
            SecondaryDebugColor = PrimaryDebugColor;

            m_stateMachine = new StateMachine(this);
            //m_stateActive = new ActiveState(m_stateMachine);
            //m_stateDying = new DyingState(m_stateMachine);
            //m_stateDead = new DeadState(m_stateMachine);

            //m_stateDying.SetLength(0.5f);
            //m_stateDying.SetExitState(m_stateDead);

            //m_stateDead.SetLength(4.0f);
            //m_stateDead.SetExitState(m_stateActive);
        }


        public override void Init()
        {
            base.Init();

        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);

        }


    }
}
