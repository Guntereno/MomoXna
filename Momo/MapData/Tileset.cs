using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MapData
{
    public class Tileset
    {
        public string Name { get; private set; }
        public string TextureName { get; private set; }

        public Texture2D DiffuseMap { get; set; }

        public void Read(Map parent, ContentReader input)
        {
            Name = input.ReadString();
            TextureName = input.ReadString();
            DiffuseMap = input.ReadExternalReference<Texture2D>();
        }
    }
}
