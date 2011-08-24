using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Fonts
{
    public class TextBatchPrinter
    {
        private const int MAX_CHARACTER_BATCH_SIZE = 3000;
        private const int MAX_PAGE = 3;

        private Effect ms_effect = null;
        private VertexPositionTexture[] ms_textBatchVertices = new VertexPositionTexture[MAX_CHARACTER_BATCH_SIZE * 4 * MAX_PAGE];
        private int[] ms_textBatchIndices = new int[MAX_CHARACTER_BATCH_SIZE * 6];
        private int[] ms_textBatchVertCnt = new int[MAX_PAGE];

        private List<TextObject> ms_tmpTextObjects = new List<TextObject>(10);


        public void Init()
        {
            int vertIndex = 0;
            for (int i = 0; i < MAX_CHARACTER_BATCH_SIZE * 6; i += 6)
            {
                ms_textBatchIndices[i + 0] = vertIndex + 0;
                ms_textBatchIndices[i + 1] = vertIndex + 1;
                ms_textBatchIndices[i + 2] = vertIndex + 2;
                ms_textBatchIndices[i + 3] = vertIndex + 2;
                ms_textBatchIndices[i + 4] = vertIndex + 3;
                ms_textBatchIndices[i + 5] = vertIndex + 0;

                vertIndex += 4;
            }

            ms_effect = null;// ShaderManager.ms_shaderManager.GetShader("text");
        }



        public void Render(List<TextObject> textObjects, Vector2 halfResolution, bool handleRenderStates, GraphicsDevice graphicsDevice)
        {
            //float viewWidth = viewport.Width;
            //float viewHeight = viewport.Height;
            //float halfViewWidth = viewport.HalfWidth;
            //float halfViewHeight = viewport.HalfHeight;
            //Vector2 texelOffset = viewport.TexelOffset;

            GlyphInfo lastGlyphInfo = null;


            //Global.GraphicsDevice.VertexDeclaration = VertexFormats.VertexPositionTextureDecl;


            if (handleRenderStates)
            {
                graphicsDevice.BlendState = BlendState.NonPremultiplied;
                graphicsDevice.DepthStencilState = DepthStencilState.None;
            }


            // Set the viewport dimension
            //EffectParameter viewHalfDimension = textShader.GetUnmanagedParameter("VIEWPORT_HALF_DIMENSION");
            //viewHalfDimension.SetValue(halfResolution);


            // Begin rendering with this effect.
            Effect effect = ms_effect;


            for (int p = 0; p < effect.CurrentTechnique.Passes.Count; p++)
            {
                //EffectPass pass = effect.CurrentTechnique.Passes[p];
                //pass.Begin();

                int textObjectCnt = textObjects.Count;
                for (int t = 0; t < textObjectCnt; ++t)
                {
                    TextObject textObject = textObjects[t];

                    float scaledLineHeight = (float)textObject.Font.m_typeface.m_lineHeight * textObject.Scale.Y;
                    Vector2 screenPosition = new Vector2(textObject.Position.X, textObject.Position.Y);
                    screenPosition.Y += textObject.GetYInset();
                    float startX = screenPosition.X;

                    int pageCnt = textObject.Font.m_typeface.m_glyphPageCnt;

                    for (int i = 0; i < pageCnt; ++i)
                        ms_textBatchVertCnt[i] = 0;


                    // Go through each line in the wordwrapper, building up a vertex array list.
                    for (int i = 0; i < textObject.WordWrapper.LineCount; ++i)
                    {
                        Line line = textObject.WordWrapper.Lines[i];
                        screenPosition.X = startX + textObject.GetXInset(i);
                        lastGlyphInfo = null;


                        for (int g = 0; g < line.m_glyphCnt; ++g)
                        {
                            GlyphInfo glyphInfo = line.m_glyphInfo[g];
                            int page = glyphInfo.m_page;


                            Vector2 screenPos = screenPosition;
                            screenPos.X += ((float)glyphInfo.m_offset.X * textObject.Scale.X);
                            screenPos.Y += ((float)glyphInfo.m_offset.Y * textObject.Scale.Y);


                            // Add any X based kerning offsets.
                            if (lastGlyphInfo != null)
                                screenPos.X += (float)lastGlyphInfo.GetKerningInfo(glyphInfo.m_character) * textObject.Scale.X;

                            Vector2 pos = new Vector2((float)screenPos.X, (float)screenPos.Y);
                            Vector2 scale = new Vector2((float)glyphInfo.m_dimension.X, (float)glyphInfo.m_dimension.Y) * textObject.Scale;

                            const float kZ = 0.0f;
                            // TODO: Get half texel offset back in.
                            //Vector3 pos3 = new Vector3(pos + texelOffset, z);
                            Vector3 pos3 = new Vector3(pos, kZ);
                            int vertOffSet = (MAX_CHARACTER_BATCH_SIZE * 4 * page) + ms_textBatchVertCnt[page];
                            ms_textBatchVertices[vertOffSet].Position = pos3;
                            ms_textBatchVertices[vertOffSet++].TextureCoordinate = glyphInfo.m_uvs[0];

                            pos3.X += scale.X;
                            ms_textBatchVertices[vertOffSet].Position = pos3;
                            ms_textBatchVertices[vertOffSet++].TextureCoordinate = glyphInfo.m_uvs[1];

                            pos3.Y += scale.Y;
                            ms_textBatchVertices[vertOffSet].Position = pos3;
                            ms_textBatchVertices[vertOffSet++].TextureCoordinate = glyphInfo.m_uvs[2];

                            pos3.X = pos.X;
                            ms_textBatchVertices[vertOffSet].Position = pos3;
                            ms_textBatchVertices[vertOffSet++].TextureCoordinate = glyphInfo.m_uvs[3];

                            ms_textBatchVertCnt[page] += 4;


                            screenPosition.X += ((float)glyphInfo.m_advance * textObject.Scale.X);

                            lastGlyphInfo = glyphInfo;
                        }

                        screenPosition.Y += scaledLineHeight;
                    }



                    // Set the text objects colours.
                    //textShader.GetUnmanagedParameter("COLOUR1").SetValue(textObject.Colour.ToVector4());
                    //textShader.GetUnmanagedParameter("COLOUR2").SetValue(textObject.OutlineColour.ToVector4());


                    // Draw our batch of tris.
                    for (int i = 0; i < pageCnt; ++i)
                    {
                        // Do not try and draw 0 primitives, it will fail and complain.
                        if (ms_textBatchVertCnt[i] == 0)
                            continue;

                        //textShader.ManagedParameterList[(int)ParameterSemantic.Type.kTexture1].SetValue(textObject.Font.m_typeface.m_glyphPageArray[i].m_texture);
                        effect.CurrentTechnique.Passes[0].Apply();


                        // Send the quad to the graphics card
                        graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(
                            PrimitiveType.TriangleList,
                            ms_textBatchVertices,
                            MAX_CHARACTER_BATCH_SIZE * 4 * i,
                            ms_textBatchVertCnt[i],
                            ms_textBatchIndices,
                            0,
                            ms_textBatchVertCnt[i] / 2);
                    }
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
