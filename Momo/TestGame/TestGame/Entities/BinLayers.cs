using System;



namespace Game.Entities
{
    class BinLayers
    {
        public const int kPlayerEntity = 0;
        public const int kEnemyEntities = 1;
        public const int kCivilianEntities = 2;
        public const int kCorpse = 3;
        public const int kBullet = 4;
        public const int kPathNodes = 5;
        public const int kBoundary = 6;

        public const int kBoundaryOcclusionSmall = 7;
        public const int kBoundaryObstructionSmall = 8;

        public const int kLayerCount = kBoundaryObstructionSmall + 1;


        public static int[] kEnemyList = new int[] { kEnemyEntities };
        public static int[] kFriendyList = new int[] { kPlayerEntity, kCivilianEntities  };
    }
}
