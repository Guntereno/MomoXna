using System;
using System.Text;

using Momo.Core;



namespace Game.Time
{
    public class TimeSystem
    {
        private int mTimeLayerCapacity = 0;
        private int mTimeLayerCount = 0;
        private int[] mTimeLayerIncrements = null;
        private ulong[] mTimeLayerTotalIncrements = null;

        private int[] mCurrentTimeUnits = null;

        private int[] mTimeLayerDigitCount = null;
        private MutableString[][] mTimeLayerLabels = null;
        private MutableString[] mTimeLayerPrepend = null;
        private MutableString[] mTimeLayerAppend = null;
        private Char[] mTimeLayerPadChar = null;

        private MutableString mCurrentTimeLabel = null;


        private float mIncrementDt = 1.0f;
        private float mPartialDt = 0.0f;


        private const char kNoPadChar = ' ';


        public MutableString CurrentTimeLabel
        {
            get { return mCurrentTimeLabel; }
        }

        public float SecondsPerSmallestUnit
        {
            set { mIncrementDt = value; }
        }


        public TimeSystem(int timeLayerCapacity, int currentTimeLabelMaxChars)
        {
            mTimeLayerIncrements = new int[timeLayerCapacity];
            mTimeLayerTotalIncrements = new ulong[timeLayerCapacity];
            mCurrentTimeUnits = new int[timeLayerCapacity];
            mTimeLayerDigitCount = new int[timeLayerCapacity];

            mTimeLayerLabels = new MutableString[timeLayerCapacity][];
            mTimeLayerPrepend = new MutableString[timeLayerCapacity];
            mTimeLayerAppend = new MutableString[timeLayerCapacity];
            mTimeLayerPadChar = new Char[timeLayerCapacity];

            mCurrentTimeLabel = new MutableString(currentTimeLabelMaxChars);

            mTimeLayerCapacity = timeLayerCapacity;
        }


        public void CreateTimeLayer(int timeIncrements, string[] unitLabels, string unitPrepend, string unitAppend)
        {
            CreateTimeLayer(timeIncrements, kNoPadChar, unitLabels, unitPrepend, unitAppend);
        }


        public void CreateTimeLayer(int timeIncrements, char padChar, string[] unitLabels, string unitPrepend, string unitAppend)
        {
            int digitCount = CountDigits(timeIncrements);
            mTimeLayerIncrements[mTimeLayerCount] = timeIncrements;
            mTimeLayerDigitCount[mTimeLayerCount] = digitCount;

            mTimeLayerPadChar[mTimeLayerCount] = padChar;

            if (unitPrepend != null)
            {
                mTimeLayerPrepend[mTimeLayerCount] = new MutableString(unitPrepend);
            }

            if (unitAppend != null)
            {
                mTimeLayerAppend[mTimeLayerCount] = new MutableString(unitAppend);
            }


            ulong totalIncrements = 1;
            for(int i = 0; i < mTimeLayerCount; ++i)
            {
                totalIncrements *= (ulong)mTimeLayerIncrements[i];
            }

            mTimeLayerTotalIncrements[mTimeLayerCount] = totalIncrements;

            if (unitLabels != null)
            {
                int labelCount = unitLabels.Length;

                mTimeLayerLabels[mTimeLayerCount] = new MutableString[labelCount];

                for (int i = 0; i < labelCount; ++i)
                {
                    mTimeLayerLabels[mTimeLayerCount][i] = new MutableString(unitLabels[i]);
                }
            }

            ++mTimeLayerCount;

            UpdateTimeLabel(mCurrentTimeUnits);
        }


        public void SetTimeLayer(int timeUnit, int timeLayer)
        {
            mCurrentTimeUnits[timeLayer] = timeUnit;
            UpdateTimeLabel(mCurrentTimeUnits);
        }


        public void Update(float dt)
        {
            mPartialDt += dt;

            while (mPartialDt >= mIncrementDt)
            {
                mPartialDt -= mIncrementDt;

                IncrementTime();
            }
        }



        private void IncrementTime()
        {
            for (int i = 0; i < mTimeLayerCount; ++i)
            {
                ++mCurrentTimeUnits[i];

                if (mCurrentTimeUnits[i] >= mTimeLayerIncrements[i])
                {
                    mCurrentTimeUnits[i] = 0;
                }
                else
                {
                    break;
                }
            }


            UpdateTimeLabel(mCurrentTimeUnits);
        }


        public void UpdateTimeLabel(int[] timeUnits)
        {
            mCurrentTimeLabel.Clear();

            for (int i = mTimeLayerCount - 1; i >= 0; --i)
            {
                if (mTimeLayerPrepend[i] != null)
                {
                    mCurrentTimeLabel.Append(mTimeLayerPrepend[i]);
                }

                if (mTimeLayerPadChar[mTimeLayerCount] != kNoPadChar)
                {
                    mCurrentTimeLabel.Append(timeUnits[i], mTimeLayerDigitCount[i], mTimeLayerPadChar[i]);
                }
                else
                {
                    mCurrentTimeLabel.Append(timeUnits[i]);
                }

                if (mTimeLayerAppend[i] != null)
                {
                    mCurrentTimeLabel.Append(mTimeLayerAppend[i]);
                }
            }

            mCurrentTimeLabel.EndAppend();
        }


        private static int CountDigits(int num)
        {
            int count = 0;
            while (num != 0)
            {
                ++count;
                num /= 10;
            }

            return count;
        }
    }
}
