﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.Spatial;
using Momo.Debug;



namespace Momo.Core.GameEntities
{
    public abstract class BaseEntity : BinItem
    {
        // --------------------------------------------------------------------
        // -- Protected Members
        // --------------------------------------------------------------------
        protected Vector2 m_position = Vector2.Zero;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public virtual void SetPosition(Vector2 position)
        {
            m_position = position;
        }


        public override Vector2 GetPosition()
        {
            return m_position;
        }


        public virtual void Update(ref FrameTime frameTime, int updateToken)
        {

        }


        public virtual void PostUpdate()
        {

        }


        public virtual void DebugRender(DebugRenderer debugRenderer)
        {

        }
    }
}
