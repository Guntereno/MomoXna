using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TmxProcessorLib.Content
{
    class Enemy
    {
        enum Species
        {
            Melee = 0,
            Missile = 1
        }

        public int m_type = 0;

        internal virtual void ImportXmlNode(System.Xml.XmlNode node, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            string type = node.Attributes["type"].Value;
            m_type = (int)(Enum.Parse(typeof(Species), type, true));
        }

        internal virtual void Write(Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentWriter output)
        {
            output.Write(m_type);
        }
    }
}
