using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Core.ObjectPools;
using Momo.Core.Spatial;
using TestGame.Entities.Players;
using TestGame.Input;

namespace TestGame.Systems
{
    public class PlayerManager
    {
        private const int kMaxPlayers = 2;

        private GameWorld m_world;
        private Bin m_bin;

        private Pool<PlayerEntity> m_players = new Pool<PlayerEntity>(kMaxPlayers, 1);

        private Vector2 m_cachedAveragePosition = new Vector2();
        private bool m_averageCached = false;



        public Pool<PlayerEntity> Players       { get { return m_players; } }



        public PlayerManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;

            m_players.RegisterPoolObjectType(typeof(PlayerEntity), kMaxPlayers);
        }

        public void Load()
        {

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


            PlayerEntity player = new PlayerEntity(m_world);

            player.InputWrapper = input;

            player.PlayerColour = debugColours[m_players.ActiveItemListCount];

            // Spawn at a spawn point
            MapData.Map map = m_world.Map;
            //int playerSpawnIndex = m_world.Random.Next(map.PlayerSpawnPoints.Length);
            int playerSpawnIndex = m_players.ActiveItemListCount;
            player.SetPosition(map.PlayerSpawnPoints[playerSpawnIndex]);
            player.Init();

            // Add to the pool and bin
            m_players.AddItem(player, true);
            player.AddToBin(m_bin);

            return player;
        }

        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < m_players.ActiveItemListCount; ++i)
            {
                m_players[i].Update(ref frameTime, i);
                m_players[i].UpdateBinEntry();
            }
            m_averageCached = false;
        }

        public Vector2 GetAveragePosition()
        {
            if (!m_averageCached)
            {
                // Calculate the center point of the players
                m_cachedAveragePosition = Vector2.Zero;
                int playerCount = m_players.ActiveItemListCount;
                for (int i = 0; i < playerCount; ++i)
                {
                    m_cachedAveragePosition += m_players[i].GetPosition();
                }
                m_cachedAveragePosition *= 1.0f / playerCount;

                m_averageCached = true;

            }

            return m_cachedAveragePosition;
        }
    }
}
