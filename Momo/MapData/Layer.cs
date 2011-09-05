using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MapData
{
    public class Layer
    {
        public string Name { get; private set; }
        public Point Dimensions { get; private set; }

        public virtual void Read(Map parent, ContentReader input)
        {
            Name = input.ReadString();
            Dimensions = input.ReadObject<Point>();
        }
    }
}
