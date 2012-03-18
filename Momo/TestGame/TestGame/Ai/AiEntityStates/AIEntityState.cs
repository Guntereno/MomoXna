using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.StateMachine;

using Game.Entities;




namespace Game.Ai.AiEntityStates
{
    public abstract class AiEntityState : State
    {
        #region Constructor

        public AiEntityState(AiEntity entity)
            : base(entity)
        {
            DebugColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
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

            AiEntity.SecondaryDebugColor = DebugColor;
        }

        #endregion
    }
}
