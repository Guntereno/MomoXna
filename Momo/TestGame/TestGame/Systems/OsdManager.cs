using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Momo.Core.ObjectPools;
using Momo.Core.Spatial;
using Momo.Fonts;
using Momo.Core;
using Momo.Debug;

using TestGame.Entities;




namespace TestGame.Systems
{
    public class OsdManager
    {
        const int kMaxPlayers = 4;

        private GameWorld m_world = null;

        private Font m_font = null;
        private TextObject[] m_weaponInfo = new TextObject[kMaxPlayers];
        private MutableString[] m_weaponString = new MutableString[kMaxPlayers];


        public OsdManager(GameWorld world)
        {
            m_world = world;

            m_font = TestGame.Instance().Content.Load<Font>("fonts/Calibri_26_b_o3");

            for (int i = 0; i < kMaxPlayers; ++i)
            {
                m_weaponInfo[i] = new TextObject("", m_font, 500, 100, 3);
                m_weaponString[i] = new MutableString(40);
            }

            m_weaponInfo[0].Position = new Vector2(25.0f, 25.0f);
        }


        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < m_world.GetPlayerCount(); ++i)
            {
                Weapons.Weapon currentWeapon = m_world.GetPlayers()[i].GetCurrentWeapon();

                m_weaponString[i].Clear();

                if (currentWeapon != null)
                {
                    m_weaponString[i].Append(currentWeapon.ToString());
                    m_weaponString[i].Append("\n");
                    m_weaponString[i].Append(currentWeapon.GetAmmoInClip());
                    m_weaponString[i].Append("\n");
                    m_weaponString[i].Append(currentWeapon.GetCurrentStateName());
                }

                m_weaponString[i].EndAppend();
                m_weaponInfo[i].SetText(m_weaponString[i].GetCharacterArray());
            }
        }


        public void Render()
        {
            for (int i = 0; i < kMaxPlayers; ++i)
            {
                m_world.GetTextPrinter().AddToDrawList(m_weaponInfo[i]);
            }
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
           
        }
    }
}
