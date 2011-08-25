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
        public class Params
        {
            public Params(float reloadTime, int clipSize, float velocity, float fireRate)
            {
                m_reloadTime = reloadTime;
                m_clipSize = clipSize;
                m_velocity = velocity;
                m_fireRate = fireRate;
            }

            public float m_reloadTime; // seconds
            public int m_clipSize;
            public float m_velocity;
            public float m_fireRate; // shells/sec
        }

        public Weapon(GameWorld world)
        {
            m_world = world;
        }

        public GameWorld GetWorld() { return m_world; }

        public abstract void Init();
        public abstract void Update(ref FrameTime frameTime, float triggerState, Vector2 pos, float facing);

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
        protected Params m_params = null;

        private GameWorld m_world = null;
    }
}
