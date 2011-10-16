using TestGame.Entities;

namespace TestGame.Weapons
{
    public interface IWeaponUser
    {
        BulletEntity.Flags GetBulletFlags();

        Weapon CurrentWeapon
        {
            get;
            set;
        }
    }
}
