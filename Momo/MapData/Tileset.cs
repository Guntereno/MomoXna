using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MapData
{
    public class Tileset
    {
        public uint FirstGid { get; private set; }
        public string Name { get; private set; }
        public string TextureName { get; private set; }
        public Point TileDimensions { get; private set; }
        public Rectangle[] Tiles { get; private set; }
        public Texture2D DiffuseMap { get; set; }

        public void Read(Map parent, ContentReader input)
        {
            Name = input.ReadString();
            FirstGid = input.ReadUInt32();
            TextureName = input.ReadString();
            TileDimensions = input.ReadObject<Point>();
            DiffuseMap = input.ReadExternalReference<Texture2D>();

            int tileCount = input.ReadInt32();
            Tiles = new Rectangle[tileCount];
            for (int i = 0; i < tileCount; i++)
            {
                Tiles[i] = input.ReadObject<Rectangle>();
            }
        }
    }
}
