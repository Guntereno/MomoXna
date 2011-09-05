using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Spatial;
using Momo.Core.ObjectPools;
using TestGame.Entities;
using Momo.Core;
using TestGame.Input;
using Microsoft.Xna.Framework;

namespace TestGame.Systems
{
    public class PlayerManager
    {
        private  GameWorld m_world;
        private  Bin m_bin;

        public static readonly int kMaxPlayers = 2;

        private Pool<PlayerEntity> m_players = new Pool<PlayerEntity>(kMaxPlayers);

        public PlayerManager(GameWorld world, Bin bin)
        {
            m_world = world;
            m_bin = bin;
        }

        public void Load()
        {
        }

        public PlayerEntity AddPlayer(InputWrapper input)
        {
            PlayerEntity player = new PlayerEntity(m_world);

            player.SetInputWrapper(input);

            Color[] debugColours = 
            {
                Color.ForestGreen,
                Color.IndianRed,
                Color.DodgerBlue,
                Color.LightYellow
            };

            player.DebugColor = debugColours[m_players.ActiveItemListCount];

            // Spawn at a spawn point
            MapData.Map map = m_world.GetMap();
            //int playerSpawnIndex = m_world.GetRandom().Next(map.PlayerSpawnPoints.Length);
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
                m_players[i].Update(ref frameTime);
                m_players[i].UpdateBinEntry();
            }
        }

        // TODO: Cache this
        public Vector2 GetAveragePosition()
        {
            // Calculate the center point of the players
            Vector2 averagePosition = Vector2.Zero;
            int playerCount = m_players.ActiveItemListCount;
            for (int i = 0; i < playerCount; ++i)
            {
                averagePosition += m_players[i].GetPosition();
            }
            averagePosition *= 1.0f / playerCount;

            return averagePosition;
        }

        public Pool<PlayerEntity> GetPlayers() { return m_players; }

    }
}
