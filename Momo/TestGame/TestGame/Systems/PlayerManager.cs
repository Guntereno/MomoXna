using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Spatial;
using Momo.Core.ObjectPools;
using TestGame.Entities;
using Momo.Core;
using TestGame.Input;

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

            // Spawn at a spawn point
            Map.Map map = m_world.GetMap();
            int playerSpawnIndex = m_world.GetRandom().Next(map.PlayerSpawnPoints.Length);
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

        public Pool<PlayerEntity> GetPlayers() { return m_players; }

    }
}
