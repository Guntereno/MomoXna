using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.ObjectPools;
using Momo.Debug;



namespace TestGame.Entities
{
    public class DynamicGameEntity : DynamicEntity, IPoolItem
    {
        private RadiusInfo m_contactRadiusInfo;
        private float m_facingAngle = 0.0f;
        private Color m_debugColor = Color.White;
        private bool m_destroyed = false;
        private TestWorld m_world;

        public DynamicGameEntity(TestWorld world)
        {
            m_world = world;
        }

        public float FacingAngle
        {
            get{ return m_facingAngle; }
            set{ m_facingAngle = value;}
        }

        public Color DebugColor
        {
            get { return m_debugColor; }
            set { m_debugColor = value; }
        }

        public TestWorld GetWorld()
        {
            return m_world;
        }

        public RadiusInfo GetContactRadiusInfo()
        {
            return m_contactRadiusInfo;
        }

        public void SetContactRadiusInfo(RadiusInfo value)
        {
            m_contactRadiusInfo = value;
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            Color fillColour = m_debugColor;
            fillColour.A = 102;
            Color outlineColour = m_debugColor * 0.2f;
            outlineColour.A = 191;

            debugRenderer.DrawCircle(GetPosition(), GetContactRadiusInfo().Radius, fillColour, outlineColour, true, 2, 8);

            Vector2 direction = new Vector2((float)Math.Sin(FacingAngle), (float)Math.Cos(FacingAngle));
            debugRenderer.DrawLine(GetPosition(), GetPosition() + (direction * m_contactRadiusInfo.Radius * 1.5f), outlineColour);
        }


        public bool IsDestroyed()
        {
            return m_destroyed;
        }

        public void DestroyItem()
        {
            m_destroyed = true;
        }

        public void ResetItem()
        {
            m_destroyed = false;
        }

    }
}
