using Microsoft.Xna.Framework;

namespace MapData
{
    public class Trigger: Object
    {
        private float m_downTime;
        private float m_triggerTime;

        public Trigger(string name, Vector2 position, float downTime, float triggerTime):
            base(name, position)
        {
            m_downTime = downTime;
            m_triggerTime = triggerTime;
        }

        public float GetDownTime() { return m_downTime; }
        public float GetTriggerTime() { return m_triggerTime; }
    }

    public class KillCountTrigger: Trigger
    {
        private int m_threshold;

        public KillCountTrigger(string name, Vector2 position, float downTime, float triggerTime, int threshold) :
            base(name, position, downTime, triggerTime)
        {
            m_threshold = threshold;
        }

        int GetThreshold() { return m_threshold; }
    }

}
