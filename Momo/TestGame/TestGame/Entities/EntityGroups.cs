using System;



namespace TestGame.Entities
{
    [Flags]
    public enum EntityGroups
    {
        None = 0x0,

        Players = 0x1,
        Enemies = 0x2,

        PlayerBullets = 0x3,
        EnemyBullets = 0x4,
        AllBullets = PlayerBullets | EnemyBullets,

        PlayerBoundaries = 0x5,
        EnemyBoundaries = 0x6,
        AllBoundaries = PlayerBoundaries | EnemyBoundaries,

        AllEntities = Players | Enemies | AllBoundaries | AllBullets
    }
}
