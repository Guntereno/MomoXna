using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapData;
using TestGame.Events;
using TestGame.Systems;
using Momo.Maths;
using Microsoft.Xna.Framework;
using Momo.Debug;

namespace TestGame.Entities
{
    public class SpawnGroup
    {
        private static readonly float kCollisionCheckPadding = 32.0f;
        private GameWorld m_world;
        private SpawnGroupData m_data;
        private SpawnEvent m_owner = null;

        internal SpawnGroup(GameWorld world, SpawnGroupData data)
        {
            m_world = world;
            m_data = data;
        }

        internal SpawnGroupData GetData() { return m_data; }

        internal void TakeOwnership(SpawnEvent owner)
        {
            System.Diagnostics.Debug.Assert(m_owner == null);
            m_owner = owner;
        }

        internal void RelinquishOwnership(SpawnEvent owner)
        {
            System.Diagnostics.Debug.Assert(m_owner == owner);
            m_owner = null;
        }

        internal bool IsOwned()
        {
            return m_owner != null;
        }

        internal float GetSquaredDistanceToPlayers()
        {
            PlayerManager playerManager = m_world.GetPlayerManager();
            Vector2 screenCenter = playerManager.GetAveragePosition();
            return (screenCenter - m_data.GetCenter()).LengthSquared();
        }

        internal void DebugRender(DebugRenderer debugRenderer)
        {
            Color debugColour = Color.Cyan;
            debugColour.A = 191;
            Color debugColourFill = Color.Cyan;
            debugColour.A = 64;

            for (int i = 0; i < m_data.GetSpawnPoints().Length; ++i)
            {
                SpawnPoint spawnPoint = m_data.GetSpawnPoints()[i];
                debugRenderer.DrawLine(m_data.GetCenter(), spawnPoint.GetPosition(), debugColour);
            }

            for (int i = 0; i < m_data.GetSpawnPoints().Length; ++i)
            {
                SpawnPoint spawnPoint = m_data.GetSpawnPoints()[i];
                const float kPointRadius = 8.0f;
                debugRenderer.DrawCircle(spawnPoint.GetPosition(), kPointRadius, debugColour, debugColour, true, 2, 8);
            }

            debugRenderer.DrawCircle(m_data.GetCenter(), m_data.GetRadius().Radius, debugColour, debugColourFill, true, 3, 14);
        }
    }
}
