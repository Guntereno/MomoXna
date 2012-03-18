using Momo.Core;
using Game.Entities;



namespace Game.Weapons
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
