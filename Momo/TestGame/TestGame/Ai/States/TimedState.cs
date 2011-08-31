using Momo.Core;
using TestGame.Entities;

namespace TestGame.Ai.States
{
    public abstract class TimedState : State
    {
        public TimedState(AiEntity entity) :
            base(entity)
        { }

        public void Init(State nextState)
        {
            m_nextState = nextState;
        }

        protected abstract float GetTime();

        public override void OnEnter()
        {
            base.OnEnter();

            m_timer = GetTime();
        }

        public override void Update(ref FrameTime frameTime)
        {
            m_timer -= frameTime.Dt;
            if (m_timer <= 0.0f)
            {
                GetEntity().SetCurrentState(m_nextState);
            }
        }

        private State m_nextState = null;
        protected float m_timer;
    }
}
