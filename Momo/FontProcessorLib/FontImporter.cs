using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;


namespace FontProcessorLib
{
    [ContentImporter(".FNT", DisplayName = "Font")]
    public class FontImporter : ContentImporter<FontContent>
    {
        public override FontContent Import(string filename, ContentImporterContext context)
        {
            //System.Diagnostics.Debugger.Launch();

            FontContent fontContent = new FontContent(filename);

            // Open the .fnt file for reading.
            FileStream fs = File.OpenRead(filename);
            StreamReader sr = new StreamReader(fs);


            string line;
            string header1 = sr.ReadLine();
            string header2 = sr.ReadLine();

            // Get the typeface name/size/boldness/lineheight.
            string typefaceName = getTagValue("face", header1).Trim('"');
            int size = int.Parse(getTagValue("size", header1));
            int boldness = int.Parse(getTagValue("bold", header1));
            int lineHeight = int.Parse(getTagValue("lineHeight", header2));

            // Get the number of pages tag from the header
            int pageCnt = int.Parse(getTagValue("pages", header2));

            if (pageCnt == 0)
                return null;

            // Get the texture sizes as specified in the .fnt file (we them that they are right!)
            float texScaleW = float.Parse(getTagValue("scaleW", header2));
            float texScaleH = float.Parse(getTagValue("scaleH", header2));

            fontContent.m_typeface.m_typefaceName = typefaceName;
            fontContent.m_typeface.m_size = size;
            fontContent.m_typeface.m_bold = (boldness > 0);
            fontContent.m_typeface.m_lineHeight = lineHeight;
            fontContent.m_typeface.m_pages = new List<FontContent.Page>(pageCnt);


            // Read in the page descriptor lines and create the page objects.
            for (int i = 0; i < pageCnt; ++i)
            {
                line = sr.ReadLine();
                string file = getTagValue("file", line);

                FontContent.Page page = new FontContent.Page();

                //string pageTextureAssetName = fontContent.m_typeface.m_typefaceName.Trim('"') + "_" + fontContent.m_typeface.m_size.ToString();
                //if (fontContent.m_typeface.m_bold)
                //    pageTextureAssetName += "_B";
                //pageTextureAssetName += "_page" + i;

                string pageTextureAssetName = System.IO.Path.GetFileNameWithoutExtension(filename) + "_0" + i;


                page.m_textureAssetName = pageTextureAssetName;
                page.m_textureFileName = file.Trim('"');

                fontContent.m_typeface.m_pages.Add(page);
            }

            // Read in the character count value
            line = sr.ReadLine();
            int charCnt = int.Parse(getTagValue("chars count", line));
            fontContent.m_typeface.m_glyphList = new List<FontContent.Glyph>(charCnt);

            // Read in the each character description
            //ex: char id=32   x=507   y=90    width=1     height=1     xoffset=0     yoffset=45    xadvance=14    page=0  chnl=15
            for (int i = 0; i < charCnt; ++i)
            {
                line = sr.ReadLine();
                string id = getTagValue("id", line);
                string x = getTagValue("x", line);
                string y = getTagValue("y", line);
                string width = getTagValue("width", line);
                string height = getTagValue("height", line);
                string xoffset = getTagValue("xoffset", line);
                string yoffset = getTagValue("yoffset", line);
                string xadvance = getTagValue("xadvance", line);
                string page = getTagValue("page", line);
                string channel = getTagValue("chnl", line);

                int iPage = int.Parse(page);
                //FontContent.Page fcPage = fontContent.m_typeface.m_pages[iPage];

                FontContent.Glyph glyph = new FontContent.Glyph();
                glyph.m_kerningList = new List<FontContent.Kerning>(10);

                glyph.m_character = (char)int.Parse(id);
                glyph.m_advance = int.Parse(xadvance);
                glyph.m_page = iPage;


                // Generate the 4 corner UVs from the x,y and width,height of the character on the sheet.
                // Top/left
                glyph.m_uvs[0] = new Vector2(float.Parse(x) / texScaleW, float.Parse(y) / texScaleH);
                // Bottom/right
                glyph.m_uvs[2] = glyph.m_uvs[0] + new Vector2(float.Parse(width) / texScaleW, float.Parse(height) / texScaleH);
                
                // Top/right
                glyph.m_uvs[1] = new Vector2(glyph.m_uvs[2].X, glyph.m_uvs[0].Y);
                // Bottom/left
                glyph.m_uvs[3] = new Vector2(glyph.m_uvs[0].X, glyph.m_uvs[2].Y);

                
                glyph.m_dimensions = new Point(int.Parse(width), int.Parse(height));
                glyph.m_offset = new Point(int.Parse(xoffset), int.Parse(yoffset));

                fontContent.m_typeface.m_glyphList.Add(glyph);
            }

            // Read in the kerning count value
            line = sr.ReadLine();

            // Check if there is any kerning infomation.
            if (line != null)
            {
                int kerningCnt = int.Parse(getTagValue("kernings count", line));

                // Read in the each kerning description
                //ex: kerning first=32  second=65  amount=-3
                for (int i = 0; i < kerningCnt; ++i)
                {
                    line = sr.ReadLine();
                    string first = getTagValue("first", line);
                    string second = getTagValue("second", line);
                    string amount = getTagValue("amount", line);

                    char iFirst = (char)int.Parse(first);
                    char iSecond = (char)int.Parse(second);
                    int iAmount = int.Parse(amount);

                    // Find the character entry for this kerning, and add the data.
                    for (int c = 0; c < charCnt; ++c)
                    {
                        FontContent.Glyph glyph = fontContent.m_typeface.m_glyphList[c];
                        if (glyph.m_character == iFirst)
                        {
                            if (glyph.m_kerningList == null)
                                glyph.m_kerningList = new List<FontContent.Kerning>(3);

                            FontContent.Kerning kerning = new FontContent.Kerning();
                            kerning.m_character = iSecond;
                            kerning.m_offset = iAmount;

                            glyph.m_kerningList.Add(kerning);
                        }
                    }
                }
            }

            sr.Close();
            fs.Close();

            return fontContent;
        }

        private string getTagValue(string tag, string text)
        {
            int tagIndex = text.IndexOf(tag);
            int equalsIndex = text.IndexOf("=", tagIndex);
            int spaceIndex = text.IndexOf(" ", equalsIndex);
            if (spaceIndex < 0)
                spaceIndex = text.Length;

            string test = text.Substring(equalsIndex + 1, spaceIndex - 1 - equalsIndex);
            return test;
        }
    }
}