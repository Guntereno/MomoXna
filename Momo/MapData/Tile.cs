using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapData
{
    public class Tile
    {
        public Texture2D DiffuseMap { get; private set; }
        public Rectangle Source { get; private set; }

        public Tile(Texture2D diffuse, Rectangle source)
        {
            DiffuseMap = diffuse;
            Source = source;
        }
    }
}
