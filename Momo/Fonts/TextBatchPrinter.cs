using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Momo.Fonts
{
    public class TextBatchPrinter
    {
        private int m_maxTextObjects = 0;
        private int m_maxCharacters = 0;
        private int m_maxPages = 0;

        private int m_textObjectCnt = 0;

        private Effect m_effect = null;
        private EffectParameter m_viewHalfDimensionParam = null;
        private EffectParameter[] m_colourParam = new EffectParameter[2];
        private EffectParameter m_fontPageTextureParam = null;

        private Vector2 m_targetResolution = Vector2.Zero;
        private Vector2 m_halfTargetResolution = Vector2.Zero;

        private TextObject[] m_textObjects = null;

        private TextVertexDeclaration[] m_textBatchVertices = null;
        private short[] m_textBatchIndices = null;
        private int[] m_textBatchVertCnt = null;




        public void Init(Effect effect, Vector2 targetResolution, int maxTextObjects, int maxCharacters, int maxPages)
        {
            m_maxTextObjects = maxTextObjects;
            m_maxCharacters = maxCharacters;
            m_maxPages = maxPages;

            m_textObjects = new TextObject[m_maxTextObjects];

            m_textBatchVertices = new TextVertexDeclaration[m_maxCharacters * 4 * m_maxPages];
            m_textBatchIndices = new short[m_maxCharacters * 6];
            m_textBatchVertCnt = new int[m_maxPages];


            int vertIndex = 0;
            for (int i = 0; i < m_maxCharacters * 6; i += 6)
            {
                m_textBatchIndices[i + 0] = (short)(vertIndex + 0);
                m_textBatchIndices[i + 1] = (short)(vertIndex + 1);
                m_textBatchIndices[i + 2] = (short)(vertIndex + 2);
                m_textBatchIndices[i + 3] = (short)(vertIndex + 2);
                m_textBatchIndices[i + 4] = (short)(vertIndex + 3);
                m_textBatchIndices[i + 5] = (short)(vertIndex + 0);

                vertIndex += 4;
            }

            m_effect = effect;
            m_viewHalfDimensionParam = m_effect.Parameters["gViewHalfDim"];
            m_colourParam[0] = m_effect.Parameters["gColour"];
            m_colourParam[1] = m_effect.Parameters["gOutlineColour"];
            m_fontPageTextureParam = m_effect.Parameters["gTex"];

            SetRenderTargetResolution(targetResolution);
        }


        public void AddToDrawList(TextObject textObject)
        {
            m_textObjects[m_textObjectCnt++] = textObject;
        }


        public void ClearDrawList()
        {
            for (int i = 0; i < m_textObjectCnt; ++i)
            {
                m_textObjects[i] = null;
            }

            m_textObjectCnt = 0;
        }


        public void SetRenderTargetResolution(int width, int height)
        {
            SetRenderTargetResolution(new Vector2((float)width, (float)height)); 
        }


        public void SetRenderTargetResolution(Vector2 resolution)
        {
            m_targetResolution = resolution;
            m_halfTargetResolution = m_targetResolution * 0.5f;
        }


        public void Render(bool handleRenderStates, GraphicsDevice graphicsDevice)
        {
            Render(m_textObjects, m_textObjectCnt, handleRenderStates, graphicsDevice);
        }


        public void Render(TextObject[] textObjects, int textObjectCnt, bool handleRenderStates, GraphicsDevice graphicsDevice)
        {
            GlyphInfo lastGlyphInfo = null;


            if (handleRenderStates)
            {
                graphicsDevice.BlendState = BlendState.NonPremultiplied;
                graphicsDevice.DepthStencilState = DepthStencilState.None;
            }


            // Set the viewport dimension
            m_viewHalfDimensionParam.SetValue(m_halfTargetResolution);


            for (int t = 0; t < textObjectCnt; ++t)
            {
                TextObject textObject = textObjects[t];

                float scaledLineHeight = (float)textObject.Font.m_typeface.m_lineHeight * textObject.Scale.Y;
                Vector2 screenPosition = new Vector2(textObject.Position.X, textObject.Position.Y);
                screenPosition.Y += textObject.GetYInset();
                float startX = screenPosition.X;

                int pageCnt = textObject.Font.m_typeface.m_glyphPageCnt;


                // Go through each line in the wordwrapper, building up a vertex array list.
                int glyphIdx = 0;
                for (int i = 0; i < textObject.WordWrapper.LineCount; ++i)
                {
                    Line line = textObject.WordWrapper.Lines[i];
                    screenPosition.X = startX + textObject.GetXInset(i);
                    lastGlyphInfo = null;


                    for (; glyphIdx <= line.m_endGlyphIdx; ++glyphIdx)
                    {
                        GlyphInfo glyphInfo = textObject.WordWrapper.GlyphInfo[glyphIdx];
                        int page = glyphInfo.m_page;


                        Vector2 screenPos = screenPosition;
                        screenPos.X += ((float)glyphInfo.m_offset.X * textObject.Scale.X);
                        screenPos.Y += ((float)glyphInfo.m_offset.Y * textObject.Scale.Y);


                        // Add any X based kerning offsets.
                        if (lastGlyphInfo != null)
                            screenPos.X += (float)lastGlyphInfo.GetKerningInfo(glyphInfo.m_character) * textObject.Scale.X;

                        Vector2 scale = new Vector2((float)glyphInfo.m_dimension.X, (float)glyphInfo.m_dimension.Y) * textObject.Scale;

                        int vertOffSet = (m_maxCharacters * 4 * page) + m_textBatchVertCnt[page];
                        m_textBatchVertices[vertOffSet].Position = screenPos;
                        m_textBatchVertices[vertOffSet++].TextureCoordinate = glyphInfo.m_uvs[0];

                        screenPos.X += scale.X;
                        m_textBatchVertices[vertOffSet].Position = screenPos;
                        m_textBatchVertices[vertOffSet++].TextureCoordinate = glyphInfo.m_uvs[1];

                        screenPos.Y += scale.Y;
                        m_textBatchVertices[vertOffSet].Position = screenPos;
                        m_textBatchVertices[vertOffSet++].TextureCoordinate = glyphInfo.m_uvs[2];

                        screenPos.X -= scale.X;
                        m_textBatchVertices[vertOffSet].Position = screenPos;
                        m_textBatchVertices[vertOffSet++].TextureCoordinate = glyphInfo.m_uvs[3];

                        m_textBatchVertCnt[page] += 4;


                        screenPosition.X += ((float)glyphInfo.m_advance * textObject.Scale.X);

                        lastGlyphInfo = glyphInfo;
                    }

                    screenPosition.Y += scaledLineHeight;
                }


                // Do not draw the text object if the next one is the same settings.
                if (t == textObjectCnt - 1 ||
                    textObjects[t + 1].Font != textObject.Font ||
                    textObjects[t + 1].Colours[0] != textObject.Colours[0] ||
                    textObjects[t + 1].Colours[1] != textObject.Colours[1])
                {
                    // Work backwards, outlines first, fill second.
                    for (int c = 1; c >= 0; --c)
                    {
                        // Only draw it if its visible.
                        if (textObject.Colours[c].A > 0)
                        {
                            // Draw our batch of tris.
                            for (int i = 0; i < pageCnt; ++i)
                            {
                                // Do not try and draw 0 primitives, it will fail and complain.
                                if (m_textBatchVertCnt[i] == 0)
                                    continue;

                                // Set the text objects colours.
                                m_colourParam[c].SetValue(textObject.Colours[c].ToVector4());

                                // Set the text page texture.
                                m_fontPageTextureParam.SetValue(textObject.Font.m_typeface.m_glyphPageArray[i].m_texture);

                                m_effect.CurrentTechnique.Passes[c].Apply();


                                // Send the quad to the graphics card
                                graphicsDevice.DrawUserIndexedPrimitives<TextVertexDeclaration>(PrimitiveType.TriangleList,
                                                                                                    m_textBatchVertices,
                                                                                                    m_maxCharacters * 4 * i,
                                                                                                    m_textBatchVertCnt[i],
                                                                                                    m_textBatchIndices,
                                                                                                    0,
                                                                                                    m_textBatchVertCnt[i] / 2);
                            }
                        }
                    }


                    for (int i = 0; i < pageCnt; ++i)
                        m_textBatchVertCnt[i] = 0;
                }
            }



            if (handleRenderStates)
            {
                graphicsDevice.BlendState = BlendState.Opaque;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }
    }



}
