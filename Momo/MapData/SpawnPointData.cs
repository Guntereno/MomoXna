using Microsoft.Xna.Framework;
using Momo.Core.Spatial;

namespace MapData
{
    public class SpawnPointData: BinItem
    {
        public Vector2 Position { get; private set; }
        public float Orientation { get; private set; }

        public override Vector2 GetPosition() { return Position; }

        public SpawnPointData(Vector2 position, float orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }
}
