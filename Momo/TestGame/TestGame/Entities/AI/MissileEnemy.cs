using Momo.Core;
using TestGame.Ai.AiEntityStates;
using TestGame.Weapons;
using Momo.Core.StateMachine;
using Microsoft.Xna.Framework;



namespace TestGame.Entities.AI
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
            m_stateFind = new FindState(this);
            m_stateGetInRange = new GetInRangeState(this);
            m_stateFireWeapon = new FireProjectileWeaponState(this);

            m_stateStunned.NextState = m_stateFind;

            m_stateFind.FoundPlayerState = m_stateGetInRange;

            m_stateGetInRange.LostPlayerState = m_stateFind;
            m_stateGetInRange.AttackState = m_stateFireWeapon;
            m_stateGetInRange.InRangeState = m_stateFireWeapon;
            m_stateGetInRange.Range = new RadiusInfo(kMinRange);

            m_stateFireWeapon.NoLongerInRangeState = m_stateGetInRange;
            m_stateFireWeapon.LostPlayerState = m_stateFind;
            m_stateFireWeapon.Range = new RadiusInfo(kMinRange);


            SecondaryDebugColor = new Color(1.0f, 0.0f, 0.0f);

            AiEntityState[] states = { m_stateFireWeapon, m_stateGetInRange, m_stateFind, m_stateStunned, m_stateDying };
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

            if (Data.GetWeapon() != MapData.Weapon.Design.None)
            {
                Weapon weapon = World.WeaponManager.Create(Data.GetWeapon());
                CurrentWeapon = weapon;
                weapon.Owner = this;
            }

            StateMachine.CurrentState = m_stateFind;
        }
    }
}
