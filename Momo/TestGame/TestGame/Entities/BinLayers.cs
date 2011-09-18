using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestGame.Entities
{
    class BinLayers
    {
        public const int kAiEntity = 0;
        public const int kBoundary = 1;
        public const int kBoundaryExtrudedSmallUnit = 2;
        public const int kBoundaryExtrudedLargeUnit = 3;

        public const int kBullet = 4;
        public const int kPathNodes = 5;

        public const int kLayerCount = kPathNodes + 1;
    }
}
