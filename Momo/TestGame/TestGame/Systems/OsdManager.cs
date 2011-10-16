using Microsoft.Xna.Framework;

using Momo.Fonts;
using Momo.Core;
using Momo.Debug;

using TestGame.Entities.Players;




namespace TestGame.Systems
{
    public class OsdManager
    {
        private const int kMaxPlayers = 4;

        private GameWorld m_world = null;

        private TextStyle m_textStyle = null;
        private TextObject[] m_weaponInfo = new TextObject[kMaxPlayers];
        private MutableString[] m_weaponString = new MutableString[kMaxPlayers];



        public OsdManager(GameWorld world)
        {
            m_world = world;

            Font font = TestGame.Instance().Content.Load<Font>("fonts/Calibri_26_b_o3");
            m_textStyle = new TextStyle(font, TextSecondaryDrawTechnique.kOutline);


            Vector2[] osdPositions = 
            {
                new Vector2(25.0f, 25.0f),
                new Vector2(25.0f, 125.0f),
                new Vector2(25.0f, 225.0f),
                new Vector2(25.0f, 325.0f)
            };

            for (int i = 0; i < kMaxPlayers; ++i)
            {
                m_weaponInfo[i] = new TextObject(m_textStyle, 500, 100, 3);
                m_weaponString[i] = new MutableString(40);

                m_weaponInfo[i].Position = osdPositions[i];
            }
        }


        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < m_world.PlayerManager.Players.ActiveItemListCount; ++i)
            {
                PlayerEntity player = m_world.PlayerManager.Players[i];
                Weapons.Weapon currentWeapon = player.GetCurrentWeapon();

                m_weaponString[i].Clear();

                if (currentWeapon != null)
                {
                    m_weaponString[i].Append(currentWeapon.ToString());
                    m_weaponString[i].Append('\n');
                    m_weaponString[i].Append(currentWeapon.GetAmmoInClip());
                    m_weaponString[i].Append('\n');
                    m_weaponString[i].Append(currentWeapon.GetCurrentStateName());
                }

                m_weaponString[i].EndAppend();
                m_weaponInfo[i].PrimaryColour = player.GetPlayerColour();
                m_weaponInfo[i].SetText(m_weaponString[i].GetCharacterArray());
            }
        }


        public void Render()
        {
            for (int i = 0; i < kMaxPlayers; ++i)
            {
                m_world.TextPrinter.AddToDrawList(m_weaponInfo[i]);
            }
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
           
        }
    }
}
