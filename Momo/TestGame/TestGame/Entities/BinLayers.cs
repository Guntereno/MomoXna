using System;



namespace TestGame.Entities
{
    class BinLayers
    {
        public const int kAiEntity = 0;
        public const int kBullet = 1;
        public const int kPathNodes = 2;
        public const int kBoundary = 3;

        public const int kBoundaryViewSmall = 4;
        public const int kBoundaryPathFindingSmall = 5;
        public const int kBoundaryNodeConnectiongSmall = 6;


        public const int kLayerCount = kBoundaryNodeConnectiongSmall + 1;
    }
}
