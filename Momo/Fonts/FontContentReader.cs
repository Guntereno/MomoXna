using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace Fonts
{
    public class FontContentReader : ContentTypeReader<Font>
    {
        protected override Font Read(ContentReader input, Font existingInstance)
        {
            Font font = new Font();

            string typefaceName = input.ReadString();
            int size = input.ReadInt32();
            bool bold = input.ReadBoolean();
            int lineHeight = input.ReadInt32();
            int pageCnt = input.ReadInt32();

            font.m_typeface.m_typefaceName = typefaceName;
            font.m_typeface.m_size = size;
            font.m_typeface.m_bold = bold;
            font.m_typeface.m_lineHeight = lineHeight;

            font.m_typeface.m_glyphPageArray = new GlyphPage[pageCnt];
            font.m_typeface.m_glyphPageCnt = pageCnt;

            for (int p = 0; p < pageCnt; ++p)
            {
                string m_textureAssetName = input.ReadString();

                string texturePath = System.IO.Path.GetDirectoryName(input.AssetName) + "\\" + m_textureAssetName;
                input.ContentManager.Load<Texture2D>(texturePath);
            }

            int glyphCnt = input.ReadInt32();
            font.m_typeface.m_glyphInfoArray = new GlyphInfo[glyphCnt];
            font.m_typeface.m_glyphInfoCnt = glyphCnt;

            for (int g = 0; g < glyphCnt; ++g)
            {
                GlyphInfo glyphInfo = new GlyphInfo();

                char character = input.ReadChar();
                int advance = input.ReadInt32();
                int page = input.ReadInt32();

                Vector2 uv1 = input.ReadVector2();
                Vector2 uv2 = input.ReadVector2();
                Vector2 uv3 = input.ReadVector2();
                Vector2 uv4 = input.ReadVector2();

                int offsetX = input.ReadInt32();
                int offsetY = input.ReadInt32();

                int dimensionX = input.ReadInt32();
                int dimensionY = input.ReadInt32();

                int kerningCnt = input.ReadInt32();


                glyphInfo.m_character = character;
                glyphInfo.m_advance = advance;
                glyphInfo.m_page = page;
                glyphInfo.m_uvs[0] = uv1;
                glyphInfo.m_uvs[1] = uv2;
                glyphInfo.m_uvs[2] = uv3;
                glyphInfo.m_uvs[3] = uv4;
                glyphInfo.m_offset = new Point(offsetX, offsetY);
                glyphInfo.m_dimension = new Point(dimensionX, dimensionY);
                glyphInfo.m_kerningCnt = kerningCnt;

                glyphInfo.m_kerningArray = new Kerning[kerningCnt];

                for (int k = 0; k < kerningCnt; ++k)
                {
                    char secondCharacter = input.ReadChar();
                    int offset = input.ReadInt32();

                    glyphInfo.m_kerningArray[k].m_character = secondCharacter;
                    glyphInfo.m_kerningArray[k].m_offset = offset;
                }

                font.m_typeface.m_glyphInfoArray[g] = glyphInfo;
            }

            font.m_typeface.PopulateFastLookup();

            return font;
        }
    }
}
