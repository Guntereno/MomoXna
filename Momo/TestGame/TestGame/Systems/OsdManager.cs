using Microsoft.Xna.Framework;

using Momo.Fonts;
using Momo.Core;
using Momo.Debug;

using Game.Entities.Players;




namespace Game.Systems
{
    public class OsdManager
    {
        private const int kMaxPlayers = 4;

        private Zone mZone = null;

        private TextStyle mTextStyle = null;
        private TextObject mTime = null;
        private TextObject[] mWeaponInfo = new TextObject[kMaxPlayers];
        private MutableString[] mWeaponString = new MutableString[kMaxPlayers];



        public OsdManager(Zone zone)
        {
            mZone = zone;

            Font font = Game.Instance.Content.Load<Font>("fonts/Calibri_26_b_o3");
            mTextStyle = new TextStyle(font, TextSecondaryDrawTechnique.kDropshadow);


            mTime = new TextObject(mTextStyle, 1000, 50, 1);
            mTime.Position = new Vector2(20.0f, 10.0f);

            Vector2[] osdPositions = 
            {
                new Vector2(20.0f, 60.0f),
                new Vector2(20.0f, 160.0f),
                new Vector2(20.0f, 260.0f),
                new Vector2(20.0f, 360.0f)
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
            mTime.SetText(mZone.World.TimeSystem.CurrentTimeLabel.GetCharacterArray());


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
            mZone.World.TextPrinter.AddToDrawList(mTime);

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
