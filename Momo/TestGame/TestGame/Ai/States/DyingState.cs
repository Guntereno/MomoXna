using Microsoft.Xna.Framework;
using TestGame.Entities;

namespace TestGame.Ai.States
{
    public class DyingState : TimedState
    {
        public DyingState(AiEntity entity) :
            base(entity)
        { }

        public override string ToString()
        {
            return "Dying (" + m_timer.ToString("F3") + ")";
        }


        public override void OnEnter()
        {
            base.OnEnter();

            GetEntity().DebugColor = new Color(0.7f, 0.5f, 0.5f, 0.5f);
        }

        public override void OnExit()
        {
            base.OnExit();

            GetEntity().Kill();
        }

        protected override float GetTime()
        {
            const float kDeathTime = 1.5f;
            return kDeathTime;
        }
    }
}
