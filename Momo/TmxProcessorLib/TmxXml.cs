using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TmxProcessorLib
{
    public class TmxXml
    {
        public TmxXml(string xml)
        {
            this.m_xml = xml;
        }

        private string m_xml;
        public string Xml { get { return m_xml; } }
    }
}
