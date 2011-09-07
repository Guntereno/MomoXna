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

namespace TmxProcessorLib.Content
{
    public class Tile
    {
        public Tile(uint id, Tileset parent, Rectangle source)
        {
            Id = id;
            Parent = parent;
            Source = source;
        }

        public uint Id { get; private set; }
        public Rectangle Source { get; private set; }
        public Tileset Parent { get; private set; }
    }

    public class Tileset
    {
        public int Index { get; private set; }
        public uint FirstGid { get; private set; }
        public string Name { get; private set; }
        public string DiffuseName { get; private set; }

        public Microsoft.Xna.Framework.Point TileDimensions { get; private set; }

        public ExternalReference<TextureContent> DiffuseMap { get; private set; }
        public ExternalReference<TextureContent> NormalMap { get; private set; }

        public List<Tile> Tiles { get; private set; }

        public float Width { get; private set; }
        public float Height { get; private set; }

        public Tileset(int index)
        {
            Index = index;
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
        }

        private string GetFileFullPath(TmxData parent, string filename)
        {
            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(parent.FileName.Remove(parent.FileName.LastIndexOf('\\')), filename)));
        }
        private string GetAssetName(string filename)
        {
            return filename.Remove(filename.LastIndexOf('.')).Substring(Directory.GetCurrentDirectory().Length + 1);
        }

        public void Process(TmxData parent, ContentProcessorContext context)
        {
            // build the path using the TileSetDirectory relative to the Content root directory
            string diffusePath = GetFileFullPath(parent, DiffuseName);

            context.Logger.LogMessage("PARENT FILENAME: {0}", parent.FileName);
            context.Logger.LogMessage("TEXTURE NAME: {0}", DiffuseName);
            context.Logger.LogMessage("PATH: {0}", diffusePath);

            // build the asset as an external reference
            OpaqueDataDictionary data = new OpaqueDataDictionary();
            data.Add("GenerateMipmaps", false);
            data.Add("ResizeToPowerOfTwo", false);
            data.Add("TextureFormat", TextureProcessorOutputFormat.Color);

            DiffuseMap = context.BuildAsset<TextureContent, TextureContent>(
                new ExternalReference<TextureContent>(diffusePath),
                "TextureProcessor",
                data,
                "TextureImporter",
                GetAssetName(diffusePath));

            // Check for the existance of the other maps
            int pointPos = diffusePath.LastIndexOf('.');

            // load the image so we can compute the individual tile source rectangles
            Width = 0;
            Height = 0;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(diffusePath))
            {
                Width = image.Width;
                Height = image.Height;
            }

            // figure out how many frames fit on the X axis
            int columns = 1;
            while (columns * TileDimensions.X < Width)
            {
                columns++;
            }

            // figure out how many frames fit on the Y axis
            int rows = 1;
            while (rows * TileDimensions.Y < Height)
            {
                rows++;
            }

            // make our tiles. tiles are numbered by row, left to right.
            Tiles = new List<Tile>();
            uint curId = FirstGid;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    new Microsoft.Xna.Framework.Rectangle();

                    int rx = x * TileDimensions.X;
                    int ry = y * TileDimensions.Y;
                    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(rx, ry, TileDimensions.X, TileDimensions.Y);

                    Tile tile = new Tile(curId, this, rect);
                    Tiles.Add(tile);

                    parent.Tiles[tile.Id] = tile;

                    ++curId;
                }
            }
        }

        public void Write(ContentWriter output)
        {
            output.Write(Name);
            output.Write(DiffuseName);
            output.WriteExternalReference(DiffuseMap);
        }

    }
}
