using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.ObjectPools;
using Momo.Debug;
using Momo.Maths;



namespace Game.Entities
{

    public class LivingGameEntity : GameEntity
    {
        private const float kDefaultHealth = 100.0f;

        private float m_maxHealth = kDefaultHealth;
        private float m_health = kDefaultHealth;


        public float MaxHealth
        {
            get { return m_maxHealth; }
            set { m_maxHealth = value; }
        }

        public float Health
        {
            get { return m_health; }
            set { m_health = value; }
        }


        public LivingGameEntity(Zone zone) :
            base(zone)
        {
 
        }


        public override void ResetItem()
        {
            base.ResetItem();

            Health = MaxHealth;
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            base.DebugRender(debugRenderer);


            const float kBarWidth = 20.0f;
            const float kBarHeight = 4.0f;

            if (m_health < m_maxHealth)
            {
                // Render health bar
                Vector2 start = GetPosition();
                Vector2 end = GetPosition();
                end.X += kBarWidth;
                Vector2 healthEnd = GetPosition();
                healthEnd.X += kBarWidth * (m_health / m_maxHealth);

                Vector2 offset = new Vector2(-4.0f, 16.0f);
                debugRenderer.DrawFilledLine(start + offset, end + offset, Color.Black, kBarHeight);
                debugRenderer.DrawFilledLine(start + offset, healthEnd + offset, new Color(0.0f, 0.7f, 0.0f, 0.7f), kBarHeight - 2.0f);
            }
        }
    }

}
