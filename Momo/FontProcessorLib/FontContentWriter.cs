using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;



namespace FontProcessorLib
{
    [ContentTypeWriter]
    public class FontContentWriter : ContentTypeWriter<FontContent>
    {
        protected override void Write(ContentWriter output, FontContent value)
        {
            // Write out the typeface name/size/boldness/lineheight
            output.Write(value.m_typeface.m_typefaceName);
            output.Write(value.m_typeface.m_size);
            output.Write(value.m_typeface.m_bold);
            output.Write(value.m_typeface.m_lineHeight);

            int pageCnt = value.m_typeface.m_pages.Count;
            output.Write(pageCnt);

            for (int p = 0; p < pageCnt; ++p)
            {
                FontContent.Page page = value.m_typeface.m_pages[p];
                output.Write(page.m_textureAssetName);
            }


            int glyphCnt = value.m_typeface.m_glyphList.Count;
            output.Write(glyphCnt);

            for (int g = 0; g < glyphCnt; ++g)
            {
                FontContent.Glyph glyph = value.m_typeface.m_glyphList[g];

                output.Write(glyph.m_character);
                output.Write(glyph.m_advance);
                output.Write(glyph.m_page);
                output.Write(glyph.m_uvs[0]);
                output.Write(glyph.m_uvs[1]);
                output.Write(glyph.m_uvs[2]);
                output.Write(glyph.m_uvs[3]);
                output.Write(glyph.m_offset.X);
                output.Write(glyph.m_offset.Y);
                output.Write(glyph.m_dimensions.X);
                output.Write(glyph.m_dimensions.Y);


                // Finally write out the kerning data for this glyph
                int kerningCnt = 0;
                if(glyph.m_kerningList != null)
                    kerningCnt = glyph.m_kerningList.Count;
                
                output.Write(kerningCnt);

                for (int k = 0; k < kerningCnt; ++k)
                {
                    FontContent.Kerning kerning = glyph.m_kerningList[k];

                    output.Write(kerning.m_character);
                    output.Write(kerning.m_offset);
                }
            }
        }


        // Tells the content pipeline what CLR type the custom
        // data will be loaded into at runtime.
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Momo.Fonts.Font, Momo.Fonts";
        }


        // Tells the content pipeline what worker type
        // will be used to load the basic model data.
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Momo.Fonts.FontContentReader, Momo.Fonts";
        }
    }
}
