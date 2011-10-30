using System;



namespace TestGame.Entities
{
    class BinLayers
    {
        public const int kPlayerEntity = 0;
        public const int kEnemyEntities = 1;
        public const int kCorpse = 2;
        public const int kBullet = 3;
        public const int kPathNodes = 4;
        public const int kBoundary = 5;

        public const int kBoundaryOcclusionSmall = 6;
        public const int kBoundaryObstructionSmall = 7;

        public const int kLayerCount = kBoundaryObstructionSmall + 1;
    }
}
