using Momo.Core;
using TestGame.Entities;



namespace TestGame.Weapons
{
    public interface IWeaponUser
    {
        Flags BulletGroupMembership { get; }

        Weapon CurrentWeapon
        {
            get;
            set;
        }
    }
}
