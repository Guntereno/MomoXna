using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Triggers;
using Momo.Core.ObjectPools;
using Momo.Core;

namespace TestGame.Events
{
    public abstract class Event
    {
        private Zone mZone;

        private bool mIsActive = false;

        private MapData.EventData mData = null;

        private Trigger mEndTrigger = null;


        public Zone Zone                        { get { return mZone; } }
        public MapData.EventData EventData      { get { return mData; } }



        public Event(Zone zone, MapData.EventData data)
        {
            mZone = zone;
            mData = data;

            if (mData.GetEndTrigger() != null)
            {
                mEndTrigger = zone.TriggerManager.RegisterTrigger(mData.GetEndTrigger());
            }
        }

        public bool GetIsActive() { return mIsActive; }

        public virtual void Begin()
        {
            Trigger.Log("Event {0} began", mData.GetName());
            mIsActive = true;
        }

        public abstract void Update(ref FrameTime frameTime);


        protected virtual void End()
        {
            Trigger.Log("Event {0} ended", mData.GetName());

            mIsActive = false;

            if (mEndTrigger != null)
            {
                mEndTrigger.Activate();
            }
        }
    }
}
