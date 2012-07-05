using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MomoMapProcessorLib.Content
{
    public class Tileset
    {
        internal string Name { get; set; }
        internal string DiffuseName { get; set; }
        internal ExternalReference<TextureContent> DiffuseMap { get; set; }

        public void Write(ContentWriter output)
        {
            output.Write(Name);
            output.Write(DiffuseName);
            output.WriteExternalReference(DiffuseMap);
        }
    }
}
