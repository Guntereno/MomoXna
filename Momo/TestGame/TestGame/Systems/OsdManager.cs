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

        private Zone mZone = null;

        private TextStyle mTextStyle = null;
        private TextObject[] mWeaponInfo = new TextObject[kMaxPlayers];
        private MutableString[] mWeaponString = new MutableString[kMaxPlayers];



        public OsdManager(Zone zone)
        {
            mZone = zone;

            Font font = TestGame.Instance.Content.Load<Font>("fonts/Calibri_26_b_o3");
            mTextStyle = new TextStyle(font, TextSecondaryDrawTechnique.kDropshadow);


            Vector2[] osdPositions = 
            {
                new Vector2(25.0f, 25.0f),
                new Vector2(25.0f, 125.0f),
                new Vector2(25.0f, 225.0f),
                new Vector2(25.0f, 325.0f)
            };

            for (int i = 0; i < kMaxPlayers; ++i)
            {
                mWeaponInfo[i] = new TextObject(mTextStyle, 500, 100, 3);
                mWeaponString[i] = new MutableString(40);

                mWeaponInfo[i].Position = osdPositions[i];
            }
        }


        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < mZone.PlayerManager.Players.ActiveItemListCount; ++i)
            {
                PlayerEntity player = mZone.PlayerManager.Players[i];
                Weapons.Weapon currentWeapon = player.CurrentWeapon;

                mWeaponString[i].Clear();

                if (currentWeapon != null)
                {
                    mWeaponString[i].Append(currentWeapon.ToString());
                    mWeaponString[i].Append('\n');
                    mWeaponString[i].Append(currentWeapon.AmmoInClip);
                    mWeaponString[i].Append('\n');
                    mWeaponString[i].Append(currentWeapon.StateMachine.CurrentStateName);
                }

                mWeaponString[i].EndAppend();
                mWeaponInfo[i].PrimaryColour = player.PlayerColour;
                mWeaponInfo[i].SetText(mWeaponString[i].GetCharacterArray());
            }
        }


        public void Render()
        {
            for (int i = 0; i < kMaxPlayers; ++i)
            {
                mZone.World.TextPrinter.AddToDrawList(mWeaponInfo[i]);
            }
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
           
        }
    }
}
