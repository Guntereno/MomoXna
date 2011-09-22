using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public class NamedData
    {
        public NamedData(string name)
        {
            m_name = name;
        }

        public string GetName()
        {
            return m_name;
        }

        private string m_name;
    }
}
