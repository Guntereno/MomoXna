using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.GameEntities;
using Momo.Core;

namespace TestGame.Entities
{
	public class DynamicGameEntity : DynamicEntity
	{
		public float FacingAngle{
			get{ return m_facingAngle; }
			set{ m_facingAngle = value;}
		}

		public RadiusInfo GetContactRadiusInfo()
		{
			return m_contactRadiusInfo;
		}

		public void SetContactRadiusInfo(RadiusInfo value)
		{
			m_contactRadiusInfo = value;
		}


		private RadiusInfo m_contactRadiusInfo;
		private float m_facingAngle = 0.0f;
	}
}
