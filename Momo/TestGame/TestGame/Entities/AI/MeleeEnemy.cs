using TestGame.Ai.AiEntityStates;
using TestGame.Weapons;
using Momo.Core.StateMachine;
using Microsoft.Xna.Framework;

namespace TestGame.Entities.AI
{
    public class MeleeEnemy : EnemyEntity
    {
        #region State Machine
        private RandomWanderState m_stateRandomWander = null;
        private FindState m_stateFind = null;
        private ChargeState m_stateCharge = null;
        private SwingMeleeWeapon m_stateAttack = null;
        #endregion

        public MeleeEnemy(GameWorld world)
            : base(world)
        {
            m_stateRandomWander = new RandomWanderState(this);
            m_stateFind = new FindState(this);
            m_stateCharge = new ChargeState(this);
            m_stateAttack = new SwingMeleeWeapon(this);

            m_stateStunned.NextState = m_stateRandomWander;

            m_stateRandomWander.FoundPlayerState = m_stateCharge;

            m_stateFind.FoundPlayerState = m_stateCharge;

            m_stateCharge.LostPlayerState = m_stateFind;
            m_stateCharge.AttackState = m_stateAttack;

            m_stateAttack.Init(m_stateCharge);

            SecondaryDebugColor = new Color(1.0f, 0.0f, 0.0f);

            AiEntityState[] states = { m_stateAttack, m_stateCharge, m_stateFind, m_stateRandomWander, m_stateStunned, m_stateDying };
            for (int i = 0; i < states.Length; ++i)
            {
                float t = (float)i / (float)states.Length;
                states[i].DebugColor = Color.Lerp(SecondaryDebugColor, Color.DarkGray, t);
            }
        }

        public override void Init(MapData.EnemyData data)
        {
            base.Init(data);

            System.Diagnostics.Debug.Assert(CurrentWeapon == null);

            // Default to the melee weapon
            MapData.Weapon.Design weaponDesign = Data.GetWeapon();
            if(weaponDesign == MapData.Weapon.Design.None)
                weaponDesign = MapData.Weapon.Design.Melee;

            Weapon weapon = World.WeaponManager.Create(weaponDesign);
            CurrentWeapon = weapon;
            weapon.Owner = this;

            StateMachine.CurrentState = m_stateFind;
        }

    }
}
