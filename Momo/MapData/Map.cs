using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace Map
{
    public class Map
    {
        public enum TypeId
        {
            Orthogonal = 0,
            Isometric = 1
        }

        public TypeId Type { get; private set; }
        public Point Dimensions { get; private set; }
        public Point TileDimensions { get; private set; }

        public Tileset[] Tilesets { get; private set; }
        public TileLayer[] TileLayers { get; private set; }

        public Tile[] Tiles { get; private set; }

        public List<Light> Lights { get; private set; }

        public float LightFactor { get; set; }

        public void Read(ContentReader input)
        {
            LightFactor = 1.0f;

            Type = (TypeId)input.ReadByte();
            Dimensions = input.ReadObject<Point>();
            TileDimensions = input.ReadObject<Point>();

            LightFactor = input.ReadSingle();

            int tilesetCount = input.ReadInt32();
            Tilesets = new Tileset[tilesetCount];
            for (int i = 0; i < tilesetCount; i++)
            {
                Tilesets[i] = new Tileset();
                Tilesets[i].Read(this, input);
            }

            int tileLayerCount = input.ReadInt32();
            TileLayers = new TileLayer[tileLayerCount];
            for (int i = 0; i < tileLayerCount; i++)
            {
                TileLayers[i] = new TileLayer();
                TileLayers[i].Read(this, input);
            }

            // Determine the size of the tile array
            uint maxId = 0;
            for (int i = 0; i < tilesetCount; i++)
            {
                uint endId = Tilesets[i].FirstGid + (uint)Tilesets[i].Tiles.Length;
                if (endId > maxId)
                    maxId = endId;
            }

            // Now build the indexed list of tiles
            Tiles = new Tile[maxId];
            for (int i = 0; i < tilesetCount; i++)
            {
                for (int j = 0; j < Tilesets[i].Tiles.Length; j++)
                {
                    Tiles[Tilesets[i].FirstGid + j] = new Tile(Tilesets[i].DiffuseMap, Tilesets[i].HeightMap, Tilesets[i].NormalMap, Tilesets[i].Tiles[j]);
                }
            }

            // Read in the lights
            int numLights = input.ReadInt32();
            Lights = new List<Light>();
            for (int i = 0; i < numLights; i++)
            {
                Light.Type type = (Light.Type)(input.ReadInt32());
                string name = input.ReadString();
                Color colour = input.ReadColor();
                switch (type)
                {
                    case Light.Type.PointLight:
                        PointLight pointLight = new PointLight(name);
                        pointLight.Colour = colour;
                        pointLight.Position = input.ReadVector3();
                        pointLight.ConstantAttenuation = input.ReadSingle();
                        pointLight.LinearAttenuation = input.ReadSingle();
                        pointLight.QuadraticAttenuation = input.ReadSingle();
                        pointLight.Radius = input.ReadSingle();
                        pointLight.Strength = input.ReadSingle();
                        Lights.Add(pointLight);
                        break;

                    default:
                        Debug.Assert(false, "Invalid light type", "Light type of {0} is not valid!", type);
                        break;
                }
            }

        }
    }

    public class Tileset
    {
        public uint FirstGid { get; private set; }
        public string Name { get; private set; }
        public string TextureName { get; private set; }
        public Point TileDimensions { get; private set; }
        public Rectangle[] Tiles { get; private set; }
        public Texture2D DiffuseMap { get; set; }
        public Texture2D HeightMap { get; set; }
        public Texture2D NormalMap { get; set; }

        public void Read(Map parent, ContentReader input)
        {
            Name = input.ReadString();
            FirstGid = input.ReadUInt32();
            TextureName = input.ReadString();
            TileDimensions = input.ReadObject<Point>();
            DiffuseMap = input.ReadExternalReference<Texture2D>();

            if (input.ReadBoolean())
            {
                HeightMap = input.ReadExternalReference<Texture2D>();
            }

            int tileCount = input.ReadInt32();
            Tiles = new Rectangle[tileCount];
            for (int i = 0; i < tileCount; i++)
            {
                Tiles[i] = input.ReadObject<Rectangle>();
            }
        }
    }

    public class Layer
    {
        public string Name { get; private set; }
        public Point Dimensions { get; private set; }

        public virtual void Read(Map parent, ContentReader input)
        {
            Name = input.ReadString();
            Dimensions = input.ReadObject<Point>();
        }
    }

    public class TileLayer : Layer
    {
        public int[] Indices{ get; private set; }

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

    public class Tile
    {
        public Texture2D DiffuseMap { get; private set; }
        public Texture2D HeightMap { get; private set; }
        public Texture2D NormalMap { get; private set; }
        public Rectangle Source { get; private set; }

        public Tile(Texture2D diffuse, Texture2D height, Texture2D normal, Rectangle source)
        {
            DiffuseMap = diffuse;
            HeightMap = height;
            NormalMap = normal;
            Source = source;
        }
    }
}
