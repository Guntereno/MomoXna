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


        private GameWorld m_world = null;

        private Pool<Weapon> m_weapons = new Pool<Weapon>(300, (int)MapData.Weapon.Design.Count);
        private bool m_needsCoalesce = false;


        public WeaponManager(GameWorld world)
        {
            m_world = world;

            m_weapons.RegisterPoolObjectType(typeof(Pistol), kWeaponMax[(int)MapData.Weapon.Design.Pistol]);
            m_weapons.RegisterPoolObjectType(typeof(Shotgun), kWeaponMax[(int)MapData.Weapon.Design.Shotgun]);
            m_weapons.RegisterPoolObjectType(typeof(Minigun), kWeaponMax[(int)MapData.Weapon.Design.Minigun]);
            m_weapons.RegisterPoolObjectType(typeof(MeleeWeapon), kWeaponMax[(int)MapData.Weapon.Design.Melee]);
        }

        public Weapon Create(MapData.Weapon.Design type)
        {
            Weapon weapon;

            switch (type)
            {
                case MapData.Weapon.Design.Pistol:
                    weapon = m_weapons.CreateItem(typeof(Pistol));
                    break;

                case MapData.Weapon.Design.Shotgun:
                    weapon = m_weapons.CreateItem(typeof(Shotgun));
                    break;

                case MapData.Weapon.Design.Minigun:
                    weapon = m_weapons.CreateItem(typeof(Minigun));
                    break;

                case MapData.Weapon.Design.Melee:
                    weapon = m_weapons.CreateItem(typeof(MeleeWeapon));
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
                Pistol pistol = new Pistol(m_world);
                m_weapons.AddItem(pistol, false);
            }

            // Shotguns
            for (int i = 0; i < kWeaponMax[(int)MapData.Weapon.Design.Shotgun]; ++i)
            {
                Shotgun shotgun = new Shotgun(m_world);
                m_weapons.AddItem(shotgun, false);
            }

            // Miniguns
            for (int i = 0; i < kWeaponMax[(int)MapData.Weapon.Design.Minigun]; ++i)
            {
                Minigun minigun = new Minigun(m_world);
                m_weapons.AddItem(minigun, false);
            }

            // Melee
            for (int i = 0; i < kWeaponMax[(int)MapData.Weapon.Design.Melee]; ++i)
            {
                MeleeWeapon meleeWeapon = new MeleeWeapon(m_world);
                m_weapons.AddItem(meleeWeapon, false);
            }
        }

        public void Update(ref FrameTime frameTime)
        {
            // TODO: Coallesce only the pool which needs it
            if (m_needsCoalesce)
            {
                m_weapons.CoalesceActiveList(false);

                m_needsCoalesce = false;
            }
        }

        public void TriggerCoalesce() { m_needsCoalesce = true; }
    }
}
