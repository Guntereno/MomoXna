using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace TmxProcessorLib.Data
{
    public class Tileset: INamed
    {
        public TmxData Parent { get; private set; }
        public uint FirstGid { get; private set; }
        public string Name { get; private set; }
        public string DiffuseName { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Microsoft.Xna.Framework.Point TileDimensions { get; private set; }

        public Tileset(TmxData parent)
        {
            Parent = parent;
        }

        public void ImportXmlNode(System.Xml.XmlNode tilesetNode, ContentImporterContext context)
        {
            if (tilesetNode.Attributes["name"] == null)
                throw new PipelineException("No 'name' node found in tileset", Name);
            Name = tilesetNode.Attributes["name"].Value;

            if (tilesetNode.Attributes["firstgid"] == null)
                throw new PipelineException("No 'firstgid' attribute found in tileset {0}", Name);
            FirstGid = uint.Parse(tilesetNode.Attributes["firstgid"].Value);

            Microsoft.Xna.Framework.Point tileDimensions = new Microsoft.Xna.Framework.Point();
            if (tilesetNode.Attributes["tilewidth"] == null)
                throw new PipelineException("No 'tilewidth' attribute found in tileset {0}", Name);
            tileDimensions.X = int.Parse(tilesetNode.Attributes["tilewidth"].Value);
            if (tilesetNode.Attributes["tileheight"] == null)
                throw new PipelineException("No 'tileheight' attribute found in tileset {0}", Name);
            tileDimensions.Y = int.Parse(tilesetNode.Attributes["tileheight"].Value);
            TileDimensions = tileDimensions;

            System.Xml.XmlNode imageNode = tilesetNode["image"];
            if (imageNode == null)
                throw new PipelineException("No 'image' node found in tileset {0}", Name);
            if (imageNode.Attributes["source"] == null)
                throw new PipelineException("No 'source' attribute found in tileset {0}'s image node", Name);
            DiffuseName = imageNode.Attributes["source"].Value;

            // load the image so we can compute the individual tile source rectangles
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(GetFileFullPath(DiffuseName)))
            {
                Width = image.Width;
                Height = image.Height;
            }
        }

        public string GetFileFullPath(string filename)
        {
            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(Parent.FileName.Remove(Parent.FileName.LastIndexOf('\\')), filename)));
        }
        public string GetAssetName(string filename)
        {
            return filename.Remove(filename.LastIndexOf('.')).Substring(Directory.GetCurrentDirectory().Length + 1);
        }
    }
}
