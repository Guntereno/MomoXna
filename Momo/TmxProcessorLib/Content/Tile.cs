using Microsoft.Xna.Framework;

namespace MomoMapProcessorLib.Content
{
    public class Tile
    {
        public Tile(uint id, TmxImporterLib.Data.Tileset parent, Rectangle source)
        {
            Id = id;
            Tileset = parent;
            Source = source;
        }

        public uint Id { get; private set; }
        public Rectangle Source { get; private set; }
        public TmxImporterLib.Data.Tileset Tileset { get; private set; }
    }
}
