using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MomoMapProcessorLib.Content
{
    class Enemy
    {
        public int m_type = 0;
        public int m_weapon = -1;

        internal virtual void ImportXmlNode(System.Xml.XmlNode node, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            string type = node.Attributes["type"].Value;
            m_type = (int)(Enum.Parse(typeof(MapData.EnemyData.Species), type, true));

            if (node.Attributes["weapon"] != null)
            {
                m_weapon = (int)Enum.Parse(typeof(MapData.Weapon.Design), node.Attributes["weapon"].Value, true);
            }
        }

        internal virtual void Write(Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentWriter output)
        {
            output.Write(m_type);
            output.Write(m_weapon);
        }
    }
}
