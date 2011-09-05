using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Graphics
{
    using VFormat = VertexPositionTexture;

    public class MapRenderer
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        bool m_inited = false;
        MapData.Map m_map = null;
        
        BasicEffect m_effect = null;

        int m_patchSize = 8;
        List<MapData.Patch>[] m_layers;

        SamplerState m_samplerState;

        BoundingFrustum m_viewFrustum = new BoundingFrustum(Matrix.Identity);


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void Init(MapData.Map map, GraphicsDevice graphicsDevice, int patchSize)
        {
            m_map = map;
            m_patchSize = patchSize;
            m_inited = true;

            m_effect = new BasicEffect(graphicsDevice);
            m_effect.TextureEnabled = true;
            m_effect.LightingEnabled = false;
            m_effect.VertexColorEnabled = false;

            // Build the patches
            // Each layer consists of a list of patches
            int numLayers = m_map.TileLayers.Length;
            m_layers = new List<MapData.Patch>[numLayers];
            for (int layerIdx = 0; layerIdx < numLayers; ++layerIdx)
            {
                m_layers[layerIdx] = new List<MapData.Patch>();
                MapData.TileLayer tileLayer = m_map.TileLayers[layerIdx];

                int patchCountX = tileLayer.Dimensions.X / m_patchSize;
                for (int patchX = 0; patchX < patchCountX; ++patchX)
                {
                    int patchCountY = tileLayer.Dimensions.Y / m_patchSize;
                    for (int patchY = 0; patchY < patchCountX; ++patchY)
                    {
                        MapData.Patch patch = BuildPatch(tileLayer, patchX * m_patchSize, patchY * m_patchSize);
                        if (patch != null)
                        {
                            m_layers[layerIdx].Add(patch);
                        }
                    }
                }
            }

            m_samplerState = new SamplerState();
            m_samplerState.AddressU = TextureAddressMode.Clamp;
            m_samplerState.AddressV = TextureAddressMode.Clamp;
            m_samplerState.AddressW = TextureAddressMode.Clamp;
            m_samplerState.Filter = TextureFilter.Point;
            m_samplerState.MaxAnisotropy = 0;
            m_samplerState.MaxMipLevel = 0;
            m_samplerState.MipMapLevelOfDetailBias = 0;
        }

        public void Render(Matrix viewMatrix, Matrix projMatrix, GraphicsDevice graphicsDevice)
        {
            if (!m_inited)
                return;

            Matrix modelMatrix = Matrix.CreateScale(1.0f, 1.0f, 1.0f);

            m_effect.World = modelMatrix;
            m_effect.View = viewMatrix;
            m_effect.Projection = projMatrix;

            // Create a view frustrum for culling
            m_viewFrustum.Matrix = modelMatrix * viewMatrix * projMatrix;

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = m_samplerState;

            for (int layerIdx = 0; layerIdx < m_map.TileLayers.Length; ++layerIdx)
            {
                for (int patchIdx = 0; patchIdx < m_layers[layerIdx].Count; ++patchIdx)
                {
                    MapData.Patch patch = m_layers[layerIdx][patchIdx];
                    if (m_viewFrustum.Intersects(patch.GetBoundingBox()))
                    {
                        patch.Render(m_effect, graphicsDevice);
                    }
                }
            }

            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        private struct TileInfo
        {
            public TileInfo(MapData.Tile tile, int x, int y)
            {
                m_tile = tile;
                m_x = x;
                m_y = y;
            }

            public MapData.Tile m_tile;
            public int m_x;
            public int m_y;
        };

        private MapData.Patch BuildPatch(MapData.TileLayer tileLayer, int xMin, int yMin)
        {
            // Create a dictionary of all neccessary tile info
            // indexed by the texture
            Dictionary<Texture2D, List<TileInfo>> tileDict = new Dictionary<Texture2D, List<TileInfo>>(); 
            for (int x = xMin; x < (xMin + m_patchSize); ++x)
            {
                System.Diagnostics.Debug.Assert(x < m_map.Dimensions.X);

                for (int y = yMin; y < (yMin + m_patchSize); ++y)
                {
                    System.Diagnostics.Debug.Assert(x < m_map.Dimensions.Y);

                    MapData.Tile tile = m_map.Tiles[tileLayer.Indices[x + (y * tileLayer.Dimensions.X)]];

                    if (tile == null)
                        continue;

                    if(!tileDict.ContainsKey(tile.DiffuseMap))
                    {
                        tileDict.Add(tile.DiffuseMap, new List<TileInfo>());
                    }
                    tileDict[tile.DiffuseMap].Add(new TileInfo(tile, x, y));
                }
            }

            if(tileDict.Count > 0)
            {
                MapData.Patch patch = new MapData.Patch();

                // Now iterate the dictionary, building a mesh for each texture
                foreach (Texture2D texture in tileDict.Keys)
                {
                    List<TileInfo> tileList = tileDict[texture];

                    int numVerts = tileList.Count * 6;
                    VFormat[] vertList = new VFormat[numVerts];
                    int currentVert=0;

                    for(int i=0; i<tileList.Count; ++i)
                    {
                        TileInfo tileInfo = tileList[i];

                        float left = (tileInfo.m_x * m_map.TileDimensions.X);
                        float right = ((tileInfo.m_x + 1) * m_map.TileDimensions.X);
                        float top = (tileInfo.m_y * m_map.TileDimensions.Y);
                        float bottom = ((tileInfo.m_y + 1) * m_map.TileDimensions.Y);

                        const float kEpsilon = 0.0f; // used to prevent colour bleeding
                        float texLeft = (((float)tileInfo.m_tile.Source.Left + kEpsilon) / (float)texture.Width);
                        float texRight = (((float)tileInfo.m_tile.Source.Right - kEpsilon) / (float)texture.Width);
                        float texTop = (((float)tileInfo.m_tile.Source.Top + kEpsilon) / (float)texture.Height);
                        float texBottom = (((float)tileInfo.m_tile.Source.Bottom - kEpsilon) / (float)texture.Height);

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

                    patch.AddMesh(texture, vertList);
                }

                return patch;
            }
            else
            {
                return null;
            }
        }

        // --------------------------------------------------------------------
        // -- Private Classes
        // --------------------------------------------------------------------

    }
}
