using System;



namespace TestGame.Entities
{
    class BinLayers
    {
        public const int kPlayerEntity = 0;
        public const int kAiEntity = 1;
        public const int kBullet = 2;
        public const int kPathNodes = 3;
        public const int kBoundary = 4;
        public const int kCorpse = 5;

        public const int kBoundaryOcclusionSmall = 6;
        public const int kBoundaryObstructionSmall = 7;

        public const int kLayerCount = kBoundaryObstructionSmall + 1;
    }
}
