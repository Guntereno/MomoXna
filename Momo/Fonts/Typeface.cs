using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Fonts
{
    public class Typeface
    {
        // Fast lookup table for chars 0->GLYPH_LOOKUP_SIZE
        public const uint GLYPH_LOOKUP_SIZE = 256;
        internal GlyphInfo[] m_fastGlyphLookup = new GlyphInfo[GLYPH_LOOKUP_SIZE];

        // GlyphInfo's are stored in unicode code order here, so can be quickly bsearch-ed
        internal GlyphInfo[] m_glyphInfoArray = null;
        internal int m_glyphInfoCnt = 0;

	    internal string m_typefaceName;
        internal int m_size;
        internal bool m_bold;
	    internal int m_lineHeight;

	    internal GlyphPage[] m_glyphPageArray = null;
	    internal int m_glyphPageCnt = 0;


        public GlyphInfo GetGlyphInfo(char character)
        {
            if (character < GLYPH_LOOKUP_SIZE)
            {
                if (m_fastGlyphLookup[character] != null)
                    return m_fastGlyphLookup[character];
            }

            return BSearchGlyphInfo(character, 0, m_glyphInfoCnt - 1);
        }


        public void PopulateFastLookup()
        {
            if(m_glyphInfoArray == null)
                return;

            for (int i = 0; i < GLYPH_LOOKUP_SIZE; ++i)
            {
                m_fastGlyphLookup[i] = null;
            }

            // We need to go through the glyphs and see if they have a character value
            // between the range of 0 to GLYPH_LOOKUP_SIZE. This allows us to look up
            // the glyph infos based on their characters very quickly (assuming they
            // are in the range 0 to GLYPH_LOOKUP_SIZE).

            // Since the glyph info's in the array are sorted by character value,
            // we only need to check the first GLYPH_LOOKUP_SIZE number of glyphs,
            // knowing that past that number it could not have a character value
            // larger.

            // HOWEVER, dont just index into m_glyphInfoArray up to GLYPH_LOOKUP_SIZE
            // as it might have less entries than that. First we calculate what index
            // we should check up to.
            uint glyphInfoCheckCnt = GLYPH_LOOKUP_SIZE;
            if (glyphInfoCheckCnt > m_glyphInfoCnt)
                glyphInfoCheckCnt = (uint)m_glyphInfoCnt;

            for (uint i = 0; i < glyphInfoCheckCnt; ++i)
            {
                int character = m_glyphInfoArray[i].m_character;
                if (character < GLYPH_LOOKUP_SIZE)
                    m_fastGlyphLookup[character] = m_glyphInfoArray[i];
                else
                    break;
            }
        }


        private GlyphInfo BSearchGlyphInfo(char character, int low, int high)
        {
            if (high < low)
                return m_glyphInfoArray[0]; // Cant find it, so just return the first entry.

            //int mid = low + ((high - low) / 2);  // Note: not (low + high) / 2 !!
            int mid = (low + high) / 2; // Faster! We arent going to overflow the int.MaxValue!
           
            if (m_glyphInfoArray[mid].m_character > character)
                return BSearchGlyphInfo(character, low, mid-1);
            else if (m_glyphInfoArray[mid].m_character < character)
                return BSearchGlyphInfo(character, mid+1, high);
            else
                return m_glyphInfoArray[mid]; // found
        }
    }
}
