using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldManager
{
	// --------------------------------------------------------------------
	// -- Public Enum
	// --------------------------------------------------------------------
	public enum WorldState
	{
		kIdle,
		kLoading,
		kLoaded,
		kActive,
		kFlushing,
		kFlushed,
	}


	// --------------------------------------------------------------------
	// --- Public Delegate Definitions
	// --------------------------------------------------------------------
	public delegate World WorldCreator();

}
