using Microsoft.Xna.Framework;

namespace TmxProcessorLib.Content
{
    public class Tile
    {
        public Tile(uint id, Data.Tileset parent, Rectangle source)
        {
            Id = id;
            Tileset = parent;
            Source = source;
        }

        public uint Id { get; private set; }
        public Rectangle Source { get; private set; }
        public Data.Tileset Tileset { get; private set; }
    }
}
