using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core
{
	public class NamedObject
	{
		// --------------------------------------------------------------------
		// -- Private Members
		// --------------------------------------------------------------------
		private string m_name = null;


		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public string Name
		{
			get { return m_name; }
		}


		public NamedObject()
		{

		}


		public NamedObject(string name)
		{
			m_name = name;
		}

	}
}
