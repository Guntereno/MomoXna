using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Momo.Core.ObjectPools;

using TestGame.Weapons;
using Momo.Core;

namespace TestGame.Systems
{
    public class WeaponManager
    {
        public readonly static int[] kWeaponMax = new int[(int)MapData.Weapon.Design.Count]
        {
            100,  // Pistol
            100,  // Shotgun
            100,  // Minigun
            100,  // Melee
        };


        private GameWorld mWorld = null;

        private Pool<Weapon> mWeapons = new Pool<Weapon>(300, (int)MapData.Weapon.Design.Count, 2, false);


        public WeaponManager(GameWorld world)
        {
            mWorld = world;

            mWeapons.RegisterPoolObjectType(typeof(Pistol), kWeaponMax[(int)MapData.Weapon.Design.Pistol]);
            mWeapons.RegisterPoolObjectType(typeof(Shotgun), kWeaponMax[(int)MapData.Weapon.Design.Shotgun]);
            mWeapons.RegisterPoolObjectType(typeof(Minigun), kWeaponMax[(int)MapData.Weapon.Design.Minigun]);
            mWeapons.RegisterPoolObjectType(typeof(MeleeWeapon), kWeaponMax[(int)MapData.Weapon.Design.Melee]);
        }

        public Weapon Create(MapData.Weapon.Design type)
        {
            Weapon weapon;

            switch (type)
            {
                case MapData.Weapon.Design.Pistol:
                    weapon = mWeapons.CreateItem(typeof(Pistol));
                    break;

                case MapData.Weapon.Design.Shotgun:
                    weapon = mWeapons.CreateItem(typeof(Shotgun));
                    break;

                case MapData.Weapon.Design.Minigun:
                    weapon = mWeapons.CreateItem(typeof(Minigun));
                    break;

                case MapData.Weapon.Design.Melee:
                    weapon = mWeapons.CreateItem(typeof(MeleeWeapon));
                    break;

                default:
                    return null;
            }

            weapon.Init();
            return weapon;
        }


        public void Load()
        {
            // Pistols
            for (int i = 0; i < kWeaponMax[(int)MapData.Weapon.Design.Pistol]; ++i)
            {
                Pistol pistol = new Pistol(mWorld);
                mWeapons.AddItem(pistol, false);
            }

            // Shotguns
            for (int i = 0; i < kWeaponMax[(int)MapData.Weapon.Design.Shotgun]; ++i)
            {
                Shotgun shotgun = new Shotgun(mWorld);
                mWeapons.AddItem(shotgun, false);
            }

            // Miniguns
            for (int i = 0; i < kWeaponMax[(int)MapData.Weapon.Design.Minigun]; ++i)
            {
                Minigun minigun = new Minigun(mWorld);
                mWeapons.AddItem(minigun, false);
            }

            // Melee
            for (int i = 0; i < kWeaponMax[(int)MapData.Weapon.Design.Melee]; ++i)
            {
                MeleeWeapon meleeWeapon = new MeleeWeapon(mWorld);
                mWeapons.AddItem(meleeWeapon, false);
            }
        }

        public void Update(ref FrameTime frameTime)
        {

        }


        public void PostUpdate()
        {
            mWeapons.Update();
        }
    }
}
