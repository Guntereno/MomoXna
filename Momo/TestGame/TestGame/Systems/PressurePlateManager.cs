using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Entities;

namespace TestGame.Systems
{
    public class PressurePlateManager
    {
        private GameWorld m_world;
        private PressurePlate[] m_pressurePlates;


        public PressurePlateManager(GameWorld gameWorld)
        {
            m_world = gameWorld;
        }

        internal void LoadPressurePoints(MapData.Map m_map)
        {
            int plateCount = m_map.PressurePlates.Length;
            m_pressurePlates = new PressurePlate[plateCount];

            for (int i = 0; i < plateCount; ++i)
            {
                MapData.PressurePlateData data = m_map.PressurePlates[i];
                Type dataType = data.GetType();
                if(dataType == typeof(MapData.PressurePlateData))
                {
                    m_pressurePlates[i] = new PressurePlate(m_world, data);
                }
                else if(dataType == typeof(MapData.InteractivePressurePlateData))
                {
                    m_pressurePlates[i] = new InteractivePressurePlate(m_world, data);
                }
            }
        }

        public void DebugRender(Momo.Debug.DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_pressurePlates.Length; ++i)
            {
                m_pressurePlates[i].DebugRender(debugRenderer);
            }
        }
    }
}
