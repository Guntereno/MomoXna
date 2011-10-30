using Momo.Core;
using TestGame.Ai.States;
using TestGame.Weapons;



namespace TestGame.Entities.Enemies
{
    public class MissileEnemy : EnemyEntity, IWeaponUser
    {
        // TODO: Make this a property of the weapon
        private const float kMinRange = 200.0f;

        #region State Machine
        private FindState m_stateFind = null;
        private GetInRangeState m_stateGetInRange = null;
        private FireProjectileWeaponState m_stateFireWeapon = null;
        #endregion



        public MissileEnemy(GameWorld world)
            : base(world)
        {
            m_stateStunned = new StunnedState(this);
            m_stateDying = new DyingState(this);
            m_stateFireWeapon = new FireProjectileWeaponState(this);

            m_stateFind = new FindState(this);
            m_stateGetInRange = new GetInRangeState(this);


            m_stateStunned.Init(m_stateFind);

            m_stateFind.Init(m_stateGetInRange);
            m_stateGetInRange.Init(m_stateFind, m_stateFireWeapon, kMinRange);
            m_stateFireWeapon.Init(m_stateGetInRange, m_stateFind, kMinRange);
        }

        public override void Init(MapData.EnemyData data)
        {
            base.Init(data);

            System.Diagnostics.Debug.Assert(CurrentWeapon == null);

            if (Data.GetWeapon() != MapData.Weapon.Design.None)
            {
                Weapon weapon = World.WeaponManager.Create(Data.GetWeapon());
                CurrentWeapon = weapon;
                weapon.Owner = this;
            }

            SetCurrentState(m_stateFind);
        }
    }
}
