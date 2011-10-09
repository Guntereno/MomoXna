using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.ObjectPools;
using Momo.Debug;
using Momo.Maths;



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

        static readonly float kDefaultHealth = 100.0f;
        protected float m_maxHealth = kDefaultHealth;
        protected float m_health = kDefaultHealth;



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

        public float GetHealth() { return m_health; }

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

            // Render health bar
            {
                const float kBarWidth = 32.0f;
                const float kBarHeight = 4.0f;
                Vector2 start = GetPosition();
                Vector2 end = GetPosition();
                end.X += kBarWidth;
                Vector2 healthEnd = GetPosition();
                healthEnd.X += kBarWidth * (m_health / m_maxHealth);

                Vector2 offset = new Vector2(-4.0f, 16.0f);
                debugRenderer.DrawFilledLine(start + offset, end + offset, Color.Black, kBarHeight);
                debugRenderer.DrawFilledLine(start + offset, healthEnd + offset, Color.Green, kBarHeight - 2.0f);
            }
        }


        public float GetRelativeFacing(Vector2 direction)
        {
            float dotFacingDirecion = Vector2.Dot(m_facingDirection, direction);
            return 0.5f + (dotFacingDirecion * 0.5f);
        }


        // Returns [0.0, 1.0] based on how close the facing is to the target. 1.0 = the same, 0.0 opposite.
        public void TurnTowards(Vector2 targetDirection, float speed)
        {
            Vector2 normalToFacing = Maths2D.Perpendicular(m_facingDirection);
            float dotNormalTargetDirection = Vector2.Dot(normalToFacing, targetDirection);

            Vector2 newFacing = m_facingDirection;

            if (dotNormalTargetDirection > 0.0f)
            {
                newFacing += (normalToFacing * speed);

                if (Vector2.Dot(Maths2D.Perpendicular(newFacing), targetDirection) < 0.0f)
                    newFacing = targetDirection;
            }
            else
            {
                newFacing -= (normalToFacing * speed);

                if (Vector2.Dot(Maths2D.Perpendicular(newFacing), targetDirection) > 0.0f)
                    newFacing = targetDirection;
            }

            newFacing.Normalize();

            FacingDirection = newFacing;
        }

        public virtual void OnCollisionEvent(ref BulletEntity bullet)
        {
        }

        protected virtual void Reset()
        {
            m_health = m_maxHealth;
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
            Reset();
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
