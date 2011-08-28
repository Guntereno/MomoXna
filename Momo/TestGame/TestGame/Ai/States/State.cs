using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Entities;
using Momo.Core;
using Microsoft.Xna.Framework;

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

    public class StunnedState : TimedState
    {
        public StunnedState(AiEntity entity) :
            base(entity)
        { }

        public override string ToString()
        {
            return "Stunned (" + m_timer.ToString("F3") + ")";
        }

        protected override float GetTime()
        {
            const float kStunTime = 0.5f;
            return kStunTime;
        }
    }


    public class DyingState : TimedState
    {
        public DyingState(AiEntity entity) :
            base(entity)
        { }

        public override string ToString()
        {
            return "Dying (" + m_timer.ToString("F3") + ")";
        }

        public override void OnExit()
        {
            GetEntity().Kill();
        }

        protected override float GetTime()
        {
            const float kDeathTime = 1.5f;
            return kDeathTime;
        }
    }

    public class RandomWanderState : State
    {
        public RandomWanderState(AiEntity entity) :
            base(entity)
        { }

        public override string ToString()
        {
            return "Random Wander";
        }

        public override void Update(ref FrameTime frameTime)
        {
            GameWorld world = GetEntity().GetWorld();
            Random random = world.GetRandom();

            float turnVelocity = GetEntity().GetTurnVelocity();
            turnVelocity += ((float)random.NextDouble() - 0.5f) * 50.0f * frameTime.Dt;
            turnVelocity = MathHelper.Clamp(turnVelocity, -1.0f, 1.0f);
            GetEntity().SetTurnVelocity(turnVelocity);

            float facingAngle = GetEntity().FacingAngle;
            facingAngle += turnVelocity * frameTime.Dt;
            GetEntity().FacingAngle = facingAngle;

            Vector2 direction = new Vector2((float)Math.Sin(facingAngle), (float)Math.Cos(facingAngle));
            Vector2 newPosition = GetEntity().GetPosition() + direction;

            GetEntity().SetPosition(newPosition);
        }
    }

}
