using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TmxProcessorLib.Content
{
    public class SpawnGroup
    {
        private ObjectGroup m_objectGroup = null;

        Vector2 m_center;
        float m_boundingRadius;
        
        public ObjectGroup GetSpawnObjects() { return m_objectGroup; }
        public Vector2 GetCenter() { return m_center; }
        public float GetBoundingRadius() { return m_boundingRadius; }


        public SpawnGroup(ObjectGroup objectGroup)
        {
            m_objectGroup = objectGroup;

            Vector2 totalVector = new Vector2();
            foreach (string spawnPointName in m_objectGroup.Objects.Keys)
            {
                Object spawnPoint = m_objectGroup.Objects[spawnPointName];
                totalVector = totalVector + spawnPoint.Position;
            }
            m_center = totalVector / m_objectGroup.Objects.Count;

            m_boundingRadius = float.MinValue;
            foreach (string spawnPointName in m_objectGroup.Objects.Keys)
            {
                Object spawnPoint = m_objectGroup.Objects[spawnPointName];
                float dist = (spawnPoint.Position - m_center).Length();
                if (dist > m_boundingRadius)
                    m_boundingRadius = dist;
            }
        }
    }
}
