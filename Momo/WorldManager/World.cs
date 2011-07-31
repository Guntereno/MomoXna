using System;
using System.Collections.Generic;
using System.Text;

namespace WorldManager
{
    // ------------------------------------------------------------------------
    // -- Class Declaration
    // ------------------------------------------------------------------------
    public abstract class World
    {
        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public abstract void Load();
        public abstract void Enter();
        public abstract void Update(float dt);
        public abstract void Exit();
        public abstract void Flush();

        public abstract void PostRender();
        public abstract void Render();
        public abstract void PreRender();

        public abstract void DebugRender();
    }
}
