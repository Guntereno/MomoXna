using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Momo.Core
{
	public struct FrameTime
	{
		// --------------------------------------------------------------------
		// -- Private Members
		// --------------------------------------------------------------------
		private float m_dt;



		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public float Dt
		{
			get { return m_dt; }
		}


		public FrameTime(float dt)
		{
			m_dt = dt;
		}
	}
}
