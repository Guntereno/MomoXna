using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;

using Momo.Debug;
using Momo.Fonts;



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
        }

        public TriggerEntity(GameWorld world, MapData.Trigger triggerData)
            : base (world)
        {
            SetName(triggerData.GetName());

            m_triggeredTime = triggerData.GetTriggerTime();
            m_downTime = triggerData.GetDownTime();

            SetPosition(triggerData.GetPosition());
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
        public void DebugRender(DebugRenderer debugRenderer, TextBatchPrinter debugTextBatchPrinter, TextStyle debugTextStyle)
        {
            UpdateDebugColour();
            UpdateDebugString();

            Vector2 worldPos2d = GetPosition();
            Vector3 worldPos = new Vector3(worldPos2d.X, worldPos2d.Y, 0.0f);
            Vector2 screenPos = GetWorld().GetCamera().GetScreenPosition(worldPos);

            debugTextBatchPrinter.AddToDrawList(m_debugString.GetCharacterArray(), DebugColor, Color.Black, screenPos, debugTextStyle);
        }
    }
}
