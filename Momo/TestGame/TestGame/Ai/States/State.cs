using Momo.Core;
using Momo.Debug;

using TestGame.Entities;



namespace TestGame.Ai.States
{
    public abstract class State
    {
        private AiEntity m_entity;


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
        public abstract void Update(ref FrameTime frameTime, int updateToken);
        public virtual void OnExit() { }

        public virtual void DebugRender(DebugRenderer debugRenderer) { }
    }
}
