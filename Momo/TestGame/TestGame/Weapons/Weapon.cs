using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Momo.Core.ObjectPools;
using Microsoft.Xna.Framework;

namespace TestGame.Weapons
{
    public abstract class Weapon : IPoolItem
    {
        public abstract void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing);

        public Weapon(TestWorld world)
        {
            m_world = world;
        }

        public TestWorld GetWorld() { return m_world; }


        public bool IsDestroyed()
        {
            return m_isDestroyed;
        }

        public void DestroyItem()
        {
            m_isDestroyed = true;
        }

        public void ResetItem()
        {
            m_isDestroyed = false;
        }

        protected bool m_isDestroyed = true;

		private TestWorld m_world = null;
    }
}
