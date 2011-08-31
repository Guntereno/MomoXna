using Microsoft.Xna.Framework;
using TestGame.Entities;

namespace TestGame.Ai.States
{
    public class StunnedState : TimedState
    {
        public StunnedState(AiEntity entity) :
            base(entity)
        { }

        public override string ToString()
        {
            return "Stunned (" + m_timer.ToString("F3") + ")";
        }

        public override void OnEnter()
        {
            base.OnEnter();

            GetEntity().DebugColor = new Color(0.5f, 0.0f, 0.0f, 0.5f);
        }

        protected override float GetTime()
        {
            const float kStunTime = 0.5f;
            return kStunTime;
        }
    }
}
