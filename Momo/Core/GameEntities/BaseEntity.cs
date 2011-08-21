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

        protected int m_flags = 0;  // Use to mark up who owns this etc. int.MaxInt flag indicates it needs destroying.


		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public abstract Vector2 GetVelocity();


        public bool NeedsDestroying()
        {
            return (m_flags == int.MaxValue);
        }


        public void SetFlags(int flag)
        {
            m_flags = flag;
        }

		public virtual void SetPosition(Vector2 position)
		{
			m_position = position;
		}


		public Vector2 GetPosition()
		{
			return m_position;
		}


		public virtual void Update(ref FrameTime frameTime)
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
