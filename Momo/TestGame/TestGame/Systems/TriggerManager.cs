using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Triggers;
using Momo.Core;

namespace Game.Systems
{
    public class TriggerManager
    {
        public const string kDefaultTriggerName = "worldStart";


        private Dictionary<string, Trigger> mTriggers = new Dictionary<string, Trigger>();
        private MutableString mLookupBuffer = new MutableString(128);
        private Trigger mDefaultTrigger = null;



        public TriggerManager()
        {
            // Default trigger called at start of stage
            mDefaultTrigger = new Trigger(kDefaultTriggerName);
            mTriggers[kDefaultTriggerName] = mDefaultTrigger;
        }

        public Trigger GetTrigger(string name)
        {
            if (mTriggers.ContainsKey(name))
            {
                return mTriggers[name];
            }

            return null;
        }

        public Trigger RegisterTrigger(string name)
        {
            if (mTriggers.ContainsKey(name))
            {
                return mTriggers[name];
            }
            else
            {
                // If the trigger doesn't exist yet, create it
                Trigger newTrigger = new Trigger(name);
                mTriggers[name] = newTrigger;
                return newTrigger;
            }
        }
    }
}
