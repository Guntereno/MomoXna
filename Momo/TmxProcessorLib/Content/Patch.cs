using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VFormat = Microsoft.Xna.Framework.Graphics.VertexPositionTexture;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace TmxProcessorLib.Content
{
    internal class Mesh
    {
        private Tileset m_tileset;
        private VFormat[] m_verts;

        public Mesh(Tileset tileset, VFormat[] verts)
        {
            m_tileset = tileset;
            m_verts = verts;
        }

        internal void Write(ContentWriter output)
        {
            output.Write(m_tileset.Index);
            output.Write(m_verts.Length);
            for (int i = 0; i < m_verts.Length; ++i)
            {
                output.WriteObject<VFormat>(m_verts[i]);
            }
        }
    }

    internal class Patch
    {
        internal List<Mesh> Meshes { get; private set; }

        public Patch()
        {
            Meshes = new List<Mesh>();
        }

        internal void Write(ContentWriter output)
        {
            output.Write(Meshes.Count);
            foreach (Mesh mesh in Meshes)
            {
                mesh.Write(output);
            }
        }


        private struct TileInfo
        {
            public TileInfo(Tile tile, int x, int y)
            {
                m_tile = tile;
                m_x = x;
                m_y = y;
            }

            public Tile m_tile;
            public int m_x;
            public int m_y;
        };

        public static Patch Build(TmxData parent, TileLayer tileLayer, int xMin, int yMin, int patchSize)
        {
            // Create a dictionary of all neccessary tile info
            // indexed by the texture
            Dictionary<Tileset, List<TileInfo>> tileDict = new Dictionary<Tileset, List<TileInfo>>();
            for (int x = xMin; x < (xMin + patchSize); ++x)
            {
                System.Diagnostics.Debug.Assert(x < parent.Dimensions.X);

                for (int y = yMin; y < (yMin + patchSize); ++y)
                {
                    System.Diagnostics.Debug.Assert(x < parent.Dimensions.Y);

                    Tile tile = tileLayer.Tiles[x + (y * tileLayer.Dimensions.X)];

                    if (tile == null)
                        continue;

                    if (!tileDict.ContainsKey(tile.Parent))
                    {
                        tileDict.Add(tile.Parent, new List<TileInfo>());
                    }
                    tileDict[tile.Parent].Add(new TileInfo(tile, x, y));
                }
            }

            if (tileDict.Count > 0)
            {
                Patch patch = new Patch();

                // Now iterate the dictionary, building a mesh for each texture
                foreach (Tileset tileset in tileDict.Keys)
                {
                    List<TileInfo> tileList = tileDict[tileset];

                    int numVerts = tileList.Count * 6;
                    VFormat[] vertList = new VFormat[numVerts];
                    int currentVert = 0;

                    for (int i = 0; i < tileList.Count; ++i)
                    {
                        TileInfo tileInfo = tileList[i];

                        float left = (tileInfo.m_x * parent.TileDimensions.X);
                        float right = ((tileInfo.m_x + 1) * parent.TileDimensions.X);
                        float top = (tileInfo.m_y * parent.TileDimensions.Y);
                        float bottom = ((tileInfo.m_y + 1) * parent.TileDimensions.Y);

                        const float kEpsilon = 0.0f; // used to prevent colour bleeding
                        float texLeft = (((float)tileInfo.m_tile.Source.Left + kEpsilon) / (float)tileset.Width);
                        float texRight = (((float)tileInfo.m_tile.Source.Right - kEpsilon) / (float)tileset.Width);
                        float texTop = (((float)tileInfo.m_tile.Source.Top + kEpsilon) / (float)tileset.Height);
                        float texBottom = (((float)tileInfo.m_tile.Source.Bottom - kEpsilon) / (float)tileset.Height);

                        vertList[currentVert].Position = new Vector3(left, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texTop);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texTop);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(left, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texBottom);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;


                        vertList[currentVert].Position = new Vector3(left, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texBottom);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texTop);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texBottom);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;
                    }

                    patch.Meshes.Add(new Mesh(tileset, vertList));
                }

                return patch;
            }
            else
            {
                return null;
            }
        }
    }


}
