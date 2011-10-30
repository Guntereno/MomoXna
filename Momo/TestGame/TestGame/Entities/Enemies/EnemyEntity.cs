using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using Momo.Core.Triggers;
using Momo.Debug;

using TestGame.Ai.States;
using TestGame.Objects;
using TestGame.Weapons;



namespace TestGame.Entities.Enemies
{
    public class EnemyEntity : AiEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        #region State Machine

        #endregion

        private MapData.EnemyData m_data = null;
        private SpawnPoint m_ownedSpawnPoint = null;



        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties

        public MapData.EnemyData Data       { get { return m_data; } }
        public Flags BulletGroupMembership  { get { return new Flags((int)EntityGroups.EnemyBullets); } }

        #endregion



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public EnemyEntity(GameWorld world)
            : base(world)
        {

        }


        public virtual void Init(MapData.EnemyData data)
        {
            Init();

            m_data = data;

            m_ownedSpawnPoint = null;
        }


        public void TakeOwnershipOf(SpawnPoint spawnPoint)
        {
            System.Diagnostics.Debug.Assert(m_ownedSpawnPoint == null);
            m_ownedSpawnPoint = spawnPoint;
            spawnPoint.TakeOwnership(this);
        }


        internal override void Kill()
        {
            base.Kill();

            if (m_ownedSpawnPoint != null)
            {
                m_ownedSpawnPoint.RelinquishOwnership(this);
                m_ownedSpawnPoint = null;
            }

            World.CorpseManager.Create(this);

            World.EnemyManager.IncrementKillCount();
        }
    }
}
