using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TmxProcessorLib.Content
{
    internal class EventGroup
    {
        public string m_name = null;
        public string m_startTrigger = null;

        internal static string AddPrefix(Stack<EventGroup> groupStack, string name)
        {
            string newName = "";
            foreach (EventGroup group in groupStack)
            {
                newName += group.m_name + ":";
            }
            newName += name;
            return newName;
        }

        internal virtual void ImportXmlNode(System.Xml.XmlNode node, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            m_name = node.Attributes["name"].Value;

            if (node.Attributes["startTrigger"] != null)
            {
                m_startTrigger = node.Attributes["startTrigger"].Value;
            }
        }
    }

    internal class Event
    {
        public string m_name = "";
        public string m_startTrigger = "";
        public string m_endTrigger = "";

        internal virtual void ImportXmlNode(System.Xml.XmlNode node, Stack<EventGroup> groupStack, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            m_name = EventGroup.AddPrefix(groupStack, node.Attributes["name"].Value);


            if (node.Attributes["startTrigger"] != null)
            {
                m_startTrigger = EventGroup.AddPrefix(groupStack, node.Attributes["startTrigger"].Value);
            }
            else if (groupStack.Peek().m_startTrigger != null)
            {
                // Group triggers are specified with absolute paths
                m_startTrigger = groupStack.Peek().m_startTrigger;
            }

            if (node.Attributes["endTrigger"] != null)
            {
                m_endTrigger = EventGroup.AddPrefix(groupStack, node.Attributes["endTrigger"].Value);
            }
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

        internal override void ImportXmlNode(System.Xml.XmlNode node, Stack<EventGroup> groupStack, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            base.ImportXmlNode(node, groupStack, context);

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
        public float m_spawnDelay = 0.0f;
        public List<Enemy> m_enemies = new List<Enemy>();

        internal override void ImportXmlNode(System.Xml.XmlNode node, Stack<EventGroup> groupStack, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            base.ImportXmlNode(node, groupStack, context);

            if (node.Attributes["timer"] != null)
                m_spawnDelay = float.Parse(node.Attributes["timer"].Value);

            XmlNodeList childNodes = node.ChildNodes;
            foreach (XmlNode childNode in childNodes)
            {
                CheckChildNode(childNode, context);
            }
        }

        private void CheckChildNode(XmlNode node, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            switch (node.Name)
            {
                case "Enemy":
                    {
                        Enemy enemy = new Enemy();
                        enemy.ImportXmlNode(node, context);
                        m_enemies.Add(enemy);
                    }
                    break;

                case "Repeat":
                    {
                        int count = int.Parse(node.Attributes["count"].Value);
                        for (int i = 0; i < count; ++i)
                        {
                            XmlNodeList childNodes = node.ChildNodes;
                            foreach (XmlNode childNode in childNodes)
                            {
                                // Recursively check this groups children
                                CheckChildNode(childNode, context);
                            }
                        }
                    }
                    break;

                default:
                    throw new System.Exception("Invalid child node type {0} found in SpawnEvent!");
            }
        }

        internal override void Write(Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentWriter output)
        {
            base.Write(output);
            output.Write(m_spawnDelay);
            output.Write(m_enemies.Count);
            foreach (Enemy enemy in m_enemies)
            {
                enemy.Write(output);
            }
        }
    }

    internal class TriggerCounterEvent : Event
    {
        public string m_countTrigger = "";
        public int m_count = 0;

        internal override void ImportXmlNode(System.Xml.XmlNode node, Stack<EventGroup> groupStack, Microsoft.Xna.Framework.Content.Pipeline.ContentImporterContext context)
        {
            base.ImportXmlNode(node, groupStack, context);

            m_countTrigger = EventGroup.AddPrefix(groupStack, node.Attributes["countTrigger"].Value);
            m_count = int.Parse(node.Attributes["count"].Value);
        }

        internal override void Write(Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentWriter output)
        {
            base.Write(output);
            output.Write(m_countTrigger);
            output.Write(m_count);
        }
    }
}
