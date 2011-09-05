using Microsoft.Xna.Framework.Content;

namespace MapData
{
    public class TileLayer : Layer
    {
        public int[] Indices { get; private set; }

        public override void Read(Map parent, ContentReader input)
        {
            base.Read(parent, input);

            int size = input.ReadInt32();
            Indices = new int[size];
            for (int i = 0; i < size; i++)
            {
                Indices[i] = input.ReadInt32();
            }
        }
    }
}
