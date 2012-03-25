using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Momo.Debug
{
    public class Profiler
    {
        // --------------------------------------------------------------------
        // -- Structs
        // --------------------------------------------------------------------
        private struct ProfileItem
        {
            public string m_name;
            public long m_startTick;
            public long m_endTick;
            public Color m_colour;
            public float m_avgMs;
        }


        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private BasicEffect m_effect = null;
        private bool m_inited = false;

        private VertexPositionColor[] m_vertices = null;
        private int[] m_indices = null;
        private float m_msPerTick;

        private ProfileItem[] m_profileItems = null;
        private int m_profileItemCnt = 0;
        private int m_profileItemCapacity = 0;
        private float m_displayRangeMs = 16.6f;

        private Vector2 m_displayTopLeft = Vector2.Zero;
        private Vector2 m_displayBottomRight = Vector2.Zero;
        private Vector2 m_displayDimension = Vector2.Zero;


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public Vector2 DisplayTopLeft
        {
            get { return m_displayTopLeft; }
            set
            { 
                m_displayTopLeft = value;
                m_displayDimension = m_displayBottomRight - m_displayTopLeft;
            }
        }

        public Vector2 DisplayBottomRight
        {
            get { return m_displayBottomRight; }
            set
            {
                m_displayBottomRight = value;
                m_displayDimension = m_displayBottomRight - m_displayTopLeft;
            }
        }

        public Vector2 DisplayDimension
        {
            set { m_displayDimension = value; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Static Public Methods
        // --------------------------------------------------------------------
        public void Init(int profileItemCapacity, GraphicsDevice graphicsDevice)
        {
            System.Diagnostics.Debug.Assert(!m_inited, "Already inited. Call DeInit first.");

            m_vertices = new VertexPositionColor[profileItemCapacity * 4];
            m_indices = new int[profileItemCapacity * 6];
            m_profileItems = new ProfileItem[profileItemCapacity];


            int vertIndex = 0;
            for (int i = 0; i < profileItemCapacity * 6; i += 6)
            {
                m_indices[i + 0] = vertIndex + 0;
                m_indices[i + 1] = vertIndex + 1;
                m_indices[i + 2] = vertIndex + 2;
                m_indices[i + 3] = vertIndex + 2;
                m_indices[i + 4] = vertIndex + 1;
                m_indices[i + 5] = vertIndex + 3;

                vertIndex += 4;
            }

            m_msPerTick = (float)(1.0f / (float)Stopwatch.Frequency) * 1000.0f;

            m_profileItemCapacity = profileItemCapacity;


            m_effect = new BasicEffect(graphicsDevice);
            m_effect.World = Matrix.Identity;
            m_effect.View = Matrix.Identity;
            m_effect.Projection = Matrix.Identity;
            m_effect.TextureEnabled = false;
            m_effect.LightingEnabled = false;
            m_effect.VertexColorEnabled = true;

            DisplayTopLeft = new Vector2(-0.97f, -0.92f);
            DisplayBottomRight = new Vector2(0.97f, -0.97f);

            m_inited = true;
        }


        public void DeInit()
        {
            System.Diagnostics.Debug.Assert(m_inited, "Init first.");

            m_effect.Dispose();
            m_effect = null;

            m_inited = false;
        }


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public Profiler()
        {

        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        // returns profileId
        public int RegisterProfileItem(string name, Color colour)
        {
            System.Diagnostics.Debug.Assert(m_profileItemCnt < m_profileItemCapacity, "Can not register any new profiles, maximum reached. (See Profiler.MAX_PROFILE_ITEMS)");
            m_profileItems[m_profileItemCnt].m_name = name;
            m_profileItems[m_profileItemCnt].m_startTick = 0;
            m_profileItems[m_profileItemCnt].m_endTick = 0;
            m_profileItems[m_profileItemCnt].m_colour = colour;
            m_profileItems[m_profileItemCnt].m_avgMs = 0.0f;

            ++m_profileItemCnt;

            return m_profileItemCnt - 1;
        }

        public void StartProfile(int profileId)
        {
            System.Diagnostics.Debug.Assert(profileId < m_profileItemCapacity, "ProfileId out of range, keep it between 0 and Profiler.MAX_PROFILE_ITEMS");
            m_profileItems[profileId].m_startTick = Stopwatch.GetTimestamp();
        }

        public void EndProfile(int profileId)
        {
            System.Diagnostics.Debug.Assert(profileId < m_profileItemCapacity, "ProfileId out of range, keep it between 0 and Profiler.MAX_PROFILE_ITEMS");
            m_profileItems[profileId].m_endTick = Stopwatch.GetTimestamp();
        }

        public void Render(GraphicsDevice graphicsDevice)
        {
            float barEnd = m_displayTopLeft.X;
            float barTop = m_displayTopLeft.Y;
            float barBottom = barTop + m_displayDimension.Y;
            int vertCnt = 0;


            if(m_profileItemCnt > 0)
            {
                for (int i = 0; i < m_profileItemCnt; ++i)
                {
                    long ticks = m_profileItems[i].m_endTick - m_profileItems[i].m_startTick;
                    float ms = ((float)ticks * m_msPerTick);
                    m_profileItems[i].m_avgMs = (0.5f * m_profileItems[i].m_avgMs) + (0.5f * ms);
                    float barLength = (m_profileItems[i].m_avgMs / m_displayRangeMs) * m_displayDimension.X;

                    m_vertices[vertCnt].Position = new Vector3(barEnd, barTop, 0.0f);
                    m_vertices[vertCnt].Color = m_profileItems[i].m_colour;
                    ++vertCnt;

                    m_vertices[vertCnt].Position = new Vector3(barEnd, barBottom, 0.0f);
                    m_vertices[vertCnt].Color = m_profileItems[i].m_colour;
                    ++vertCnt;

                    barEnd += barLength;

                    m_vertices[vertCnt].Position = new Vector3(barEnd, barTop, 0.0f);
                    m_vertices[vertCnt].Color = m_profileItems[i].m_colour;
                    ++vertCnt;

                    m_vertices[vertCnt].Position = new Vector3(barEnd, barBottom, 0.0f);
                    m_vertices[vertCnt].Color = m_profileItems[i].m_colour;
                    ++vertCnt;
                }



                graphicsDevice.BlendState = BlendState.NonPremultiplied;
                graphicsDevice.DepthStencilState = DepthStencilState.None;
                graphicsDevice.RasterizerState = RasterizerState.CullNone;


                for (int p = 0; p < m_effect.CurrentTechnique.Passes.Count; p++)
                {
                    EffectPass pass = m_effect.CurrentTechnique.Passes[p];
                    pass.Apply();

                    graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(  PrimitiveType.TriangleList,
                                                                                    m_vertices,
                                                                                    0,
                                                                                    vertCnt,
                                                                                    m_indices,
                                                                                    0,
                                                                                    vertCnt / 2);
                }


                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
                graphicsDevice.BlendState = BlendState.Opaque;
            }
        }
    }
}
