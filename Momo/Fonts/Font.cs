using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Fonts
{
    public class Font
    {
        internal Typeface m_typeface = new Typeface();


        public GlyphInfo GetGlyphInfo(char character)
        {
            return m_typeface.GetGlyphInfo(character);
        }
    }
}
