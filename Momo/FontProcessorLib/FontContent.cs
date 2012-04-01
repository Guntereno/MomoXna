using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;


namespace FontProcessorLib
{
    public class FontContent : ContentItem
    {
        // --------------------------------------------------------------------
        // -- Structs
        // --------------------------------------------------------------------
        public struct TypefaceInfo
        {
            public string m_typefaceName;
            public bool m_bold;
            public int m_size;
            public int m_lineHeight;

            public List<Page> m_pages;
            public List<Glyph> m_glyphList;
        }

        public struct Page
        {
            //public ExternalReference<TextureContent> m_texture;
            public string m_textureAssetName;
            public string m_textureFileName;
        }

        public class Glyph
        {
            public char m_character;
            public int m_advance;
            public int m_page;

            public Vector2[] m_uvs = new Vector2[4];

            public Point m_offset;
            public Point m_dimensions;

            public List<Kerning> m_kerningList;
        }

        public struct Kerning
        {
            public char m_character;
            public int m_offset;
        }


        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        public TypefaceInfo m_typeface = new TypefaceInfo();


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties

        #endregion


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public FontContent(string filename)
        {
            Identity = new ContentIdentity(filename);
        }
    }
}
