using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WorldManager
{
    public class WorldManager
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private List<WorldInfo> m_worldList = new List<WorldInfo>(10);


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void Update(float dt)
        {
            foreach(WorldInfo info in m_worldList)
            {
                switch(info.m_state)
                {
                    case WorldState.kLoading:
                        break;

                    case WorldState.kLoaded:
                        info.m_state = WorldState.kActive;
                        break;

                    case WorldState.kActive:
                        Debug.Assert(info.m_created == true, "WorldManager: Trying to update a world that has not been created.");
                        Debug.Assert(info.m_world != null, "WorldManager: Trying to update a world that is not active.");

                        info.m_world.Update(dt);
                        break;

                    case WorldState.kFlushing:
                        break;

                    case WorldState.kFlushed:
                        info.m_state = WorldState.kIdle;
                        break;
                }
                
            }
        }


        public void Render()
        {
            foreach(WorldInfo info in m_worldList)
            {
                if(info.m_state == WorldState.kActive)
                {
                    Debug.Assert(info.m_created == true, "WorldManager: Trying to render a world that has not been created.");
                    Debug.Assert(info.m_world != null, "WorldManager: Trying to render a world that is not active.");
                    
                    info.m_world.Render();
                }
            }
        }


        // Allocates memory - use wisely
        public void RegisterWorld(string name, WorldCreator worldCreaterFunc)
        {
            Debug.Assert(GetWorld(name) == null, "WorldManager: World with this name already exists.");

            WorldInfo info = new WorldInfo();
            info.m_name = name;
            info.m_state = WorldState.kIdle;
            info.m_world = null;
            info.m_created = false;
            info.m_worldCreaterFunc = worldCreaterFunc;

            m_worldList.Add(info);
        }


        public void LoadWorld(string name)
        {
            WorldInfo info = GetWorldInfo(name);

            Debug.Assert(info != null, "WorldManager: Can not find world to load.");
            Debug.Assert(info.m_state == WorldState.kIdle, "WorldManager: World is not in its idle state.");

            Thread t = new Thread(info.LoadWorldThread);
            t.Start();
        }


        public void FlushWorld(string name)
        {
            WorldInfo info = GetWorldInfo(name);

            Debug.Assert(info != null, "WorldManager: Can not find world to load.");
            Debug.Assert(info.m_state == WorldState.kActive, "WorldManager: World is not in its active state.");

            Thread t = new Thread(info.FlushWorldThread);
            t.Start();
        }


        public World GetWorld(string name)
        {
            WorldInfo info = GetWorldInfo(name);

            if(info != null)
                return info.m_world;

            return null;
        }


        public WorldState GetWorldState(string name)
        {
            WorldInfo info = GetWorldInfo(name);
            return info.m_state;
        }


        public bool IsWorldRegistered(string name)
        {
            WorldInfo info = GetWorldInfo(name);
            return (info != null);
        }


        // --------------------------------------------------------------------
        // -- Private Methods
        // --------------------------------------------------------------------
        private WorldInfo GetWorldInfo(string name)
        {
            foreach(WorldInfo info in m_worldList)
            {
                if(info.m_name == name)
                    return info;
            }

            return null;
        }
    }
}
