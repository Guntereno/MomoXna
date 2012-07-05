using Game.Entities.Players;
using Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Momo.Core;
using Momo.Core.Nodes.Cameras;
using Momo.Core.ObjectPools;
using Momo.Debug;



namespace Game.Systems
{
    public class PlayerManager
    {
        private const int kMaxPlayers = 2;

        private Zone mZone;

        private Pool<PlayerEntity> mPlayers = new Pool<PlayerEntity>(kMaxPlayers, 1, 2, false);

        private Vector2 mAveragePosition = new Vector2();

        private Model mPlayerModel = null;

        public Pool<PlayerEntity> Players       { get { return mPlayers; } }
        public Vector2 AveragePlayerPosition    { get { return mAveragePosition; } }



        public PlayerManager(Zone zone)
        {
            mZone = zone;

            mPlayers.RegisterPoolObjectType(typeof(PlayerEntity), kMaxPlayers);
        }

        public void Load()
        {
            mPlayerModel = ResourceManager.Instance.Get<Model>("models/link");
        }

        public PlayerEntity AddPlayer(InputWrapper input)
        {
            Color[] debugColours = 
            {
                Color.ForestGreen,
                Color.IndianRed,
                Color.DodgerBlue,
                Color.LightYellow
            };


            PlayerEntity player = new PlayerEntity(mZone);
            player.Model = new Momo.Core.Models.ModelInst(mPlayerModel, Matrix.Identity);

            player.InputWrapper = input;

            player.PlayerColour = debugColours[mPlayers.ActiveItemListCount];

            // Spawn at a spawn point
            MapData.MomoMap map = mZone.Map;
            //int playerSpawnIndex = m_world.Random.Next(map.PlayerSpawnPoints.Length);
            int playerSpawnIndex = mPlayers.ActiveItemListCount;
            player.SetPosition(map.PlayerSpawnPoints[playerSpawnIndex]);
            player.Init();

            // Add to the pool and bin
            mPlayers.AddItem(player, true);
            player.AddToBin(mZone.Bin);

            return player;
        }

        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < mPlayers.ActiveItemListCount; ++i)
            {
                mPlayers[i].Update(ref frameTime, 0);
                mPlayers[i].UpdateBinEntry();
            }

            UpdateAveragePosition();
        }

        public void PostUpdate()
        {
            mPlayers.Update();
        }

        public void UpdateAveragePosition()
        {
            // Calculate the center point of the players
            mAveragePosition = Vector2.Zero;
            int playerCount = mPlayers.ActiveItemListCount;
            for (int i = 0; i < playerCount; ++i)
            {
                mAveragePosition += mPlayers[i].GetPosition();
            }

            mAveragePosition /= (float)playerCount;
        }

        public void Render(CameraNode camera, GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < mPlayers.ActiveItemListCount; ++i)
            {
                mPlayers[i].Render(camera, graphicsDevice);
            }
        }

        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < mPlayers.ActiveItemListCount; ++i)
            {
                mPlayers[i].DebugRender(debugRenderer);
            }
        }
    }
}
