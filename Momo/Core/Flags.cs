using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core
{
    public struct Flags
    {
        private int mData;


        public int Data { get { return mData; } }


        public Flags(Flags flags)
        {
            mData = flags.Data;
        }

        public Flags(int flags)
        {
            mData = flags;
        }


        public void Clear()
        {
            mData = 0;
        }


        public void SetFlag( int flag )
        {
            mData &= flag;
        }


        public bool IsFlagSet(int flag)
        {
            return ( mData & flag ) == flag;
        }


        public bool AnyFlagSet(int flag)
        {
            return (mData & flag) > 0;
        }

        public bool AnyFlagSet(Flags flags)
        {
            return AnyFlagSet(flags.Data);
        }
    }
}
