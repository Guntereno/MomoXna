using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MomoMap.Content
{
    public class SpawnPoint
    {
        public Vector2 Position { get; private set; }
        public float Orientation { get; private set; }

        public SpawnPoint(Vector2 position, float orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        internal void Write(ContentWriter output)
        {
            output.Write(Position);
            output.Write(Orientation);
        }
    }
}
