using Microsoft.Xna.Framework;

namespace MapData
{
    public class SpawnPointData
    {
        public Vector2 Position { get; private set; }
        public float Orientation { get; private set; }


        public SpawnPointData(Vector2 position, float orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }
}
