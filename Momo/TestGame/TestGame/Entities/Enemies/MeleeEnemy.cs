using TestGame.Ai.States;
using TestGame.Weapons;

namespace TestGame.Entities.Enemies
{
    public class MeleeEnemy : AiEntity
    {
        private RandomWanderState m_stateRandomWander = null;
        private FindState m_stateFind = null;
        private ChargeState m_stateCharge = null;

        public MeleeEnemy(GameWorld world)
            : base(world)
        {
            m_stateStunned = new StunnedState(this);
            m_stateDying = new DyingState(this);

            m_stateRandomWander = new RandomWanderState(this);
            m_stateFind = new FindState(this);
            m_stateCharge = new ChargeState(this);


            m_stateStunned.Init(m_stateRandomWander);

            m_stateRandomWander.Init(m_stateCharge);
            m_stateFind.Init(m_stateCharge);
            m_stateCharge.Init(m_stateFind);

        }

        public override void Init(MapData.EnemyData data)
        {
            base.Init(data);

            System.Diagnostics.Debug.Assert(GetCurrentWeapon() == null);

            // Default to the melee weapon
            MapData.Weapon.Design weaponDesign = GetData().GetWeapon();
            if(weaponDesign == MapData.Weapon.Design.None)
                weaponDesign = MapData.Weapon.Design.Melee;

            Weapon weapon = GetWorld().GetWeaponManager().Create(weaponDesign);
            SetCurrentWeapon(weapon);
            weapon.SetOwner(this);

            SetCurrentState(m_stateFind);
        }

    }
}
