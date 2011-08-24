using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Fonts
{
    public class GlyphInfo
    {
        internal char m_character;
        internal int m_advance;
        internal int m_page;

        internal Vector2[] m_uvs = new Vector2[4];

        internal Point m_offset;
        internal Point m_dimension;

        internal Kerning[] m_kerningArray;
        internal int m_kerningCnt;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public int GetKerningInfo(char character)
        {
            if (m_kerningCnt == 0)
                return 0;

            return BSearchKerningInfo(character, 0, m_kerningCnt - 1);
        }

        public int BSearchKerningInfo(char character, int low, int high)
        {
            if (high < low)
                return 0;

            //int mid = low + ((high - low) / 2);  // Note: not (low + high) / 2 !!
            int mid = (low + high) / 2; // Faster! We arent going to overflow the int.MaxValue!

            if (m_kerningArray[mid].m_character > character)
                return BSearchKerningInfo(character, low, mid - 1);
            else if (m_kerningArray[mid].m_character < character)
                return BSearchKerningInfo(character, mid + 1, high);
            else
                return m_kerningArray[mid].m_offset; // found
        }
    }
}
