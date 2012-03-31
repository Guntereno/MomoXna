using System;



namespace Game.Time
{
    public struct TimeStamp
    {
        private ulong mTimeCode;


        public ulong TimeCode
        {
            get { return mTimeCode; }
            set { mTimeCode = value; }
        }
    }
}
