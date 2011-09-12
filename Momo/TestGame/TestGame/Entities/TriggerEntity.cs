using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.GameEntities;
using Momo.Core;
using Momo.Debug;
using Momo.Fonts;
using Microsoft.Xna.Framework;

namespace TestGame.Entities
{
    abstract class TriggerEntity : StaticGameEntity, INamed
    {
        public enum State
        {
            Untriggered,
            Triggered,
            Disabled
        }

        const int kMaxNameLength = 32;
        private MutableString m_name = new MutableString(kMaxNameLength);

        const int kDebugStringLength = 64;
        protected MutableString m_debugString = new MutableString(kDebugStringLength);

        private int m_nameHash = 0;

        private State m_state = State.Untriggered;

        private TextObject m_debugText = null;

        enum Flags
        {
            None = 0x0,
            Active = 0x1,
        }
        Flags m_flags = Flags.None;

        float m_stateTimer;

        float m_triggeredTime = -1.0f;
        float m_downTime = -1.0f;


        public TriggerEntity(GameWorld world, String name, float triggeredTime, float downTime)
            : base(world)
        {
            SetName(name);

            m_triggeredTime = triggeredTime;
            m_downTime = downTime;

            m_debugText = new TextObject("", TestGame.Instance().GetDebugFont(), 1024, kDebugStringLength, 1);
        }

        public TriggerEntity(GameWorld world, MapData.Trigger triggerData)
            : base (world)
        {
            SetName(triggerData.GetName());

            m_triggeredTime = triggerData.GetTriggerTime();
            m_downTime = triggerData.GetDownTime();

            m_debugText = new TextObject("", TestGame.Instance().GetDebugFont(), 800, kMaxNameLength, 1);
        }

        bool IsTriggered()
        {
            return m_state == State.Triggered;
        }

        public bool GetActive()
        {
            return (m_flags & Flags.Active) == Flags.Active;
        }

        public void SetActive(bool value)
        {
            if (value)
            {
                m_flags = (m_flags | Flags.Active);
            }
            else
            {
                m_flags = (m_flags & ~Flags.Active);
            }
        }

        public override void Update(ref FrameTime frameTime)
        {
            if (!GetActive())
            {
                return;
            }

            switch (m_state)
            {
                case State.Untriggered:
                    if (TriggerCondition())
                    {
                        m_stateTimer = m_triggeredTime;
                        m_state = State.Triggered;
                    }
                    break;

                case State.Triggered:
                    if (m_stateTimer >= 0.0f)
                    {
                        m_stateTimer -= frameTime.Dt;

                        if (m_stateTimer <= 0.0f)
                        {
                            if (m_downTime >= 0.0f)
                            {
                                m_stateTimer = m_downTime;
                                m_state = State.Disabled;
                            }
                            else
                            {
                                m_state = State.Untriggered;
                            }
                        }
                    }
                    // Otherwise we remain Triggered until unset
                    break;

                case State.Disabled:
                    if (m_stateTimer > 0.0f)
                    {
                        m_stateTimer -= frameTime.Dt;
                    }
                    else
                    {
                        m_state = State.Untriggered;
                    }
                    break;
            }
        }

        protected virtual void UpdateDebugColour()
        {
            if (!GetActive())
                DebugColor = Color.Gray;
            else
            {
                switch (m_state)
                {
                    case State.Untriggered: DebugColor = Color.Green; break;
                    case State.Triggered: DebugColor = Color.White; break;
                    case State.Disabled: DebugColor = Color.Red; break;
                }
            }
        }

        protected virtual void UpdateDebugString()
        {
            m_debugString = GetName();
        }

        abstract protected bool TriggerCondition();


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

        // --------------------------------------------------------------------
        // -- BaseEntity implementation
        // --------------------------------------------------------------------
        public override void DebugRender(DebugRenderer debugRenderer)
        {
            UpdateDebugColour();
            UpdateDebugString();

            m_debugText.SetText(m_debugString.GetCharacterArray());
            m_debugText.Colour = DebugColor;
            m_debugText.Position = GetPosition();
            GetWorld().GetTextPrinter().AddToDrawList(m_debugText);
        }
    }
}
