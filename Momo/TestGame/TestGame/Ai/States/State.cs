using Momo.Core;
using TestGame.Entities;

namespace TestGame.Ai.States
{
    public abstract class State
    {
        public State(AiEntity entity)
        {
            m_entity = entity;
        }

        public override string ToString()
        {
            return "";
        }

        public AiEntity GetEntity() { return m_entity; }

        public virtual void OnEnter() { }
        public abstract void Update(ref FrameTime frameTime);
        public virtual void OnExit() { }

        AiEntity m_entity;
    }
}
