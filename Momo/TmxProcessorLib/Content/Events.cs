using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TmxProcessorLib.Content
{
    internal class Event
    {
        public string m_name = "";
        public string m_startTrigger = "";
        public string m_endTrigger = "";

        internal virtual void ImportXmlNode(System.Xml.XmlNode node, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            m_name = node.Attributes["name"].Value;
            
            if(node.Attributes["startTrigger"] != null)
                m_startTrigger = node.Attributes["startTrigger"].Value;

            if (node.Attributes["endTrigger"] != null)
                m_endTrigger = node.Attributes["endTrigger"].Value;
        }

        internal virtual void Write(Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentWriter output)
        {
            output.Write(m_name);
            output.Write(m_startTrigger);
            output.Write(m_endTrigger);
        }
    }

    internal class TimerEvent : Event
    {
        public float m_time;

        internal override void ImportXmlNode(System.Xml.XmlNode node, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            base.ImportXmlNode(node, context);

            m_time = float.Parse(node.Attributes["time"].Value);
        }

        internal override void Write(Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentWriter output)
        {
            base.Write(output);
            output.Write(m_time);
        }
    }

    internal class SpawnEvent : Event
    {
        public int m_spawnCount;
        public float m_spawnDelay;

        internal override void ImportXmlNode(System.Xml.XmlNode node, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            base.ImportXmlNode(node, context);

            m_spawnCount = int.Parse(node.Attributes["count"].Value);
            m_spawnDelay = float.Parse(node.Attributes["timer"].Value);
        }


        internal override void Write(Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentWriter output)
        {
            base.Write(output);
            output.Write(m_spawnCount);
            output.Write(m_spawnDelay);
        }
    }
}
