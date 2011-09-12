using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.ObjectPools;
using Momo.Debug;



namespace TestGame.Entities
{
    public class DynamicGameEntity : DynamicEntity, INamed, IPoolItem
    {
        const int kMaxNameLength = 32;

        private MutableString m_name = new MutableString(kMaxNameLength);
        private int m_nameHash = 0;

        private RadiusInfo m_contactRadiusInfo;
        private float m_facingAngle = 0.0f;
        private Vector2 m_facingDirection = Vector2.Zero;

        private Color m_debugColor = Color.White;
        private bool m_destroyed = false;

        private GameWorld m_world;

        protected float m_health = 100.0f;



        public DynamicGameEntity(GameWorld world)
        {
            m_world = world;
        }

        public float FacingAngle
        {
            get{ return m_facingAngle; }
            set{
                m_facingAngle = value;
                m_facingDirection = new Vector2((float)Math.Sin(m_facingAngle), (float)Math.Cos(m_facingAngle));
            }
        }

        public Vector2 FacingDirection
        {
            get { return m_facingDirection; }
            set {
                m_facingDirection = value;
                m_facingAngle = (float)Math.Atan2(m_facingDirection.X, m_facingDirection.Y);
            }
        }

        public Color DebugColor
        {
            get { return m_debugColor; }
            set { m_debugColor = value; }
        }

        public GameWorld GetWorld()
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
            debugRenderer.DrawLine(GetPosition(), GetPosition() + (m_facingDirection * m_contactRadiusInfo.Radius * 1.5f), outlineColour);
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

        public void ResetItem()
        {
            m_destroyed = false;
        }


        // --------------------------------------------------------------------
        // -- INamed interface implementation
        // --------------------------------------------------------------------
        public void SetName(MutableString name)
        {
            m_name.Set(name);
            m_nameHash = Hashing.GenerateHash(m_name.GetCharacterArray(), m_name.GetLength());
        }

        public void SetName(string name)
        {
            m_name.Set(name);
            m_nameHash = Hashing.GenerateHash(m_name.GetCharacterArray(), m_name.GetLength());
        }

        public MutableString GetName()
        {
            return m_name;
        }

        public int GetNameHash()
        {
            return m_nameHash;
        }
    }
}
