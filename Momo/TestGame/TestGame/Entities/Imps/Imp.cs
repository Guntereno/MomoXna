using TestGame.Ai.States;


namespace TestGame.Entities.Imps
{
    public class Imp : AiEntity
    {

        public Imp(GameWorld world)
            : base(world)
        {

        }

        public override void Init(MapData.EnemyData data)
        {
            base.Init(data);

        }

    }
}
