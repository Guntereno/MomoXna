﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public struct BinLocation
    {
        public static readonly BinLocation kInvalidBinLocation = new BinLocation(BinIndex.kInvalidIndex.m_index, BinIndex.kInvalidIndex.m_index);


        internal int m_x;
        internal int m_y;


        public int X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public int Y
        {
            get { return m_y; }
            set { m_y = value; }
        }


        public BinLocation(int x, int y)
        {
            m_x = x;
            m_y = y;
        }


        public void Invalidate()
        {
            this = kInvalidBinLocation;
        }



        public static bool operator ==(BinLocation left, BinLocation right)
        {
            return (left.m_x == right.m_x) && (left.m_y == right.m_y);
        }


        public static bool operator !=(BinLocation left, BinLocation right)
        {
            return (left.m_x != right.m_x) || (left.m_y != right.m_y);
        }


        public static BinLocation operator +(BinLocation left, BinLocation right)
        {
            return new BinLocation(left.m_x + right.m_x, left.m_y + right.m_y);
        }


        public static BinLocation operator -(BinLocation left, BinLocation right)
        {
            return new BinLocation(left.m_x - right.m_x, left.m_y - right.m_y);
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BinLocation binLocation = (BinLocation)obj;

            return this == binLocation;
        }

    }
}
