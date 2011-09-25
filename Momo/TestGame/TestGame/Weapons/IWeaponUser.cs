using TestGame.Entities;

namespace TestGame.Weapons
{
    public interface IWeaponUser
    {
        BulletEntity.Flags GetBulletFlags();

        Weapon GetCurrentWeapon();
        void SetCurrentWeapon(Weapon weapon);
    }
}
