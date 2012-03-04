using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.StateMachine;

using TestGame.Entities;




namespace TestGame.Ai.AiEntityStates
{
    public abstract class AiEntityState : State
    {
        #region Constructor

        public AiEntityState(AiEntity entity)
            : base(entity)
        {
            DebugColor = Color.White;
        }

        #endregion


        #region Public Interface

        public Color DebugColor { get; set; }

        public AiEntity AiEntity
        {
            get { return Owner as AiEntity; }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            AiEntity.PrimaryDebugColor = DebugColor;
        }

        #endregion
    }
}
