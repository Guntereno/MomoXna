using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Momo.Core.ObjectPools;

using TestGame.Weapons;

namespace TestGame.Systems
{
    public class WeaponManager
    {
        public enum WeaponType
        {
            Pistol = 0,
            Shotgun,
            Minigun,

            Count
        }

        public WeaponManager(GameWorld world)
        {
            m_world = world;
        }

        public Weapon Create(WeaponType type)
        {
            Weapon weapon;

            switch (type)
            {
                case WeaponType.Pistol:
                    weapon = m_pistols.CreateItem();
                    break;

                case WeaponType.Shotgun:
                    weapon = m_shotguns.CreateItem();
                    break;

                case WeaponType.Minigun:
                    weapon = m_miniguns.CreateItem();
                    break;

                default:
                    return null;
            }

            weapon.Init();
            return weapon;
        }

        public static readonly int[] kWeaponMax = new int[(int)WeaponType.Count]
        {
            8,  // Pistol
            8,  // Shotgun
            8   // Minigun
        };

        public void Load()
        {
            // Pistols
            for (int i = 0; i < kWeaponMax[(int)WeaponType.Pistol]; ++i)
            {
                Pistol pistol = new Pistol(m_world);
                m_pistols.AddItem(pistol, false);
            }

            // Shotguns
            for (int i = 0; i < kWeaponMax[(int)WeaponType.Pistol]; ++i)
            {
                Shotgun shotgun = new Shotgun(m_world);
                m_shotguns.AddItem(shotgun, false);
            }

            // Miniguns
            for (int i = 0; i < kWeaponMax[(int)WeaponType.Minigun]; ++i)
            {
                Minigun minigun = new Minigun(m_world);
                m_miniguns.AddItem(minigun, false);
            }
        }

        private GameWorld m_world;

        private Pool<Pistol> m_pistols = new Pool<Pistol>(kWeaponMax[(int)WeaponType.Pistol]);
        private Pool<Shotgun> m_shotguns = new Pool<Shotgun>(kWeaponMax[(int)WeaponType.Shotgun]);
        private Pool<Minigun> m_miniguns = new Pool<Minigun>(kWeaponMax[(int)WeaponType.Minigun]);
    }
}
