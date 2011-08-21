using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;



namespace WorldManager
{
	internal class WorldInfo
	{
		// --------------------------------------------------------------------
		// -- Internal Members
		// --------------------------------------------------------------------
		internal string m_name;
		internal WorldState m_state;
		internal World m_world;
		internal bool m_created;
		internal WorldCreator m_worldCreaterFunc;


		// --------------------------------------------------------------------
		// -- Internal Methods
		// --------------------------------------------------------------------
		internal void CreateWorld()
		{
			Debug.Assert(m_world == null);
			m_world = m_worldCreaterFunc();

			// Check the world has sucessfully been made.
			Debug.Assert(m_world != null);
			m_created = true;
		}


		internal void LoadWorldThread()
		{
#if XBOX360
			// set the hardware thread to run on. (not 0 or 2 - reserved for XNA framework)
			Thread.CurrentThread.SetProcessorAffinity( new int[ ] { 5 } );
#endif

			// If the world has not been created, create before we load.
			if (m_created == false)
				CreateWorld();

			Debug.Assert(m_state == WorldState.kIdle);

			m_state = WorldState.kLoading;
			m_world.Load();
			m_state = WorldState.kLoaded;
		}


		internal void FlushWorldThread()
		{
#if XBOX360
			// set the hardware thread to run on. (not 0 or 2 - reserved for XNA framework)
			Thread.CurrentThread.SetProcessorAffinity( new int[ ] { 5 } );
#endif

			Debug.Assert(m_state == WorldState.kActive);

			m_state = WorldState.kFlushing;
			m_world.Flush();
			m_state = WorldState.kFlushed;
		}
	}
}
