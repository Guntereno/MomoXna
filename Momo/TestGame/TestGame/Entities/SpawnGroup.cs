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
using System.Diagnostics;

namespace TestGame.Entities
{
    public class SpawnPoint
    {
        GameWorld m_world = null;

        private SpawnPointData m_data;
        private AiEntity m_owner;

        internal SpawnPoint(GameWorld world, SpawnPointData data)
        {
            m_world = world;
            m_data = data;
        }

        internal SpawnPointData GetData() { return m_data; }

        internal float GetSquaredDistanceToPlayers()
        {
            PlayerManager playerManager = m_world.GetPlayerManager();
            Vector2 screenCenter = playerManager.GetAveragePosition();
            return (screenCenter - m_data.GetPosition()).LengthSquared();
        }

        internal void DebugRender(DebugRenderer debugRenderer)
        {
            Color debugColour = Color.Cyan;
            debugColour.A = 191;
            Color debugColourFill = Color.Cyan;
            debugColour.A = 64;

            const float kPointRadius = 8.0f;
            debugRenderer.DrawCircle(m_data.GetPosition(), kPointRadius, debugColour, debugColour, true, 2, 8);
        }

        internal bool IsOwned()
        {
            return m_owner != null;
        }

        internal void RelinquishOwnership(AiEntity owner)
        {
            Debug.Assert(m_owner == owner);
            m_owner = null;
        }

        internal void TakeOwnership(AiEntity owner)
        {
            Debug.Assert(m_owner == null);
            m_owner = owner;
        }
    }
}
