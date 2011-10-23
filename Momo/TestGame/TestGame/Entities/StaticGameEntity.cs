using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.GameEntities;
using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Debug;
using Momo.Core.ObjectPools;

namespace TestGame.Entities
{
    public class StaticGameEntity: StaticEntity, IPoolItem
    {
        private static readonly float kContactDimensionPadding = 1.0f;

        private GameWorld m_world;
        private Color m_debugColor = Color.White;

        private RadiusInfo m_contactRadiusInfo;

        private float m_facingAngle = 0.0f;
        private Vector2 m_facingDirection = Vector2.Zero;

        private bool m_destroyed = false;


        public GameWorld World { get { return m_world; } }

        public Color PrimaryDebugColour
        {
            get { return m_debugColor; }
            set { m_debugColor = value; }
        }

        public float FacingAngle
        {
            get { return m_facingAngle; }
            set
            {
                m_facingAngle = value;
                m_facingDirection = new Vector2((float)Math.Sin(m_facingAngle), (float)Math.Cos(m_facingAngle));
            }
        }

        public Vector2 FacingDirection
        {
            get { return m_facingDirection; }
            set
            {
                m_facingDirection = value;
                m_facingAngle = (float)Math.Atan2(m_facingDirection.X, m_facingDirection.Y);
            }
        }

        public RadiusInfo ContactRadiusInfo
        {
            get { return m_contactRadiusInfo; }
            set { m_contactRadiusInfo = value; }
        }


        public StaticGameEntity(GameWorld world)
        {
            m_world = world;
        }



        public static float GetContactDimensionPadding()
        {
            return kContactDimensionPadding;
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            Color fillColour = m_debugColor;
            fillColour.A = 102;
            Color outlineColour = m_debugColor * 0.2f;
            outlineColour.A = 191;

            debugRenderer.DrawCircle(GetPosition(), ContactRadiusInfo.Radius, fillColour, outlineColour, true, 2, 8);
            debugRenderer.DrawLine(GetPosition(), GetPosition() + (m_facingDirection * m_contactRadiusInfo.Radius * 1.5f), outlineColour);
        }

        protected virtual void Reset()
        {
            
        }

        // --------------------------------------------------------------------
        // -- IPool interface implementation
        // --------------------------------------------------------------------
        public bool IsDestroyed()
        {
            return m_destroyed;
        }

        public void DestroyItem()
        {
            m_destroyed = true;
        }

        public virtual void ResetItem()
        {
            m_destroyed = false;
            Reset();
        }

    }
}
