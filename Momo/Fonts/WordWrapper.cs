using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Fonts
{
    // --------------------------------------------------------------------
    // -- Struct
    // --------------------------------------------------------------------
    public struct Line
    {
        public int m_startCharacter;
        public int m_width;
        public GlyphInfo[] m_glyphInfo;
        public int m_glyphCnt;
    }


    public class WordWrapper
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Font m_font = null;
        private int m_maxWidth;

        private int m_maxCharsPerLine = 0;
        private int m_maxLines = 0;

        private Line[] m_lines = null;
        private int m_lineCnt = 0;

        private bool m_canBreakWord = false;


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public int MaxWidth
        {
            get { return m_maxWidth; }
        }

        public int MaxCharactersPerLine
        {
            get { return m_maxCharsPerLine; }
        }

        public int MaxLines
        {
            get { return m_maxLines; }
        }

        public Line[] Lines
        {
            get { return m_lines; }
        }

        public int LineCount
        {
            get { return m_lineCnt; }
        }

        public bool CanBreakWord
        {
            get { return m_canBreakWord; }
            set { m_canBreakWord = value; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public WordWrapper(int maxCharsPerLine, int maxLines)
        {
            m_maxCharsPerLine = maxCharsPerLine;
            m_maxLines = maxLines;

            m_lines = new Line[maxLines];
            for (int i = 0; i < maxLines; ++i)
            {
                m_lines[i].m_glyphInfo = new GlyphInfo[maxCharsPerLine];
                m_lines[i].m_glyphCnt = 0;
            }
        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void SetText(string text)
        {
            SetText(text, m_font, m_maxWidth);
        }

        public void SetText(string text, Font font, int width)
        {
            m_font = font;
            m_maxWidth = width;

            int characterIndex = 0;
            int stringLen = text.Length;
            m_lineCnt = 0;
            while ((characterIndex < stringLen) && (m_lineCnt < m_maxLines))
            {
                FillLine(ref m_lines[m_lineCnt], text, stringLen, ref characterIndex, m_maxWidth);
                ++m_lineCnt;
            }
        }

        //public void SetTextPart(string text, Font font, int width, int startChar, int endChar, int lineStart)
        //{
        //    m_font = font;
        //    m_maxWidth = width;

        //    int characterIndex = startChar;
        //    int stringLen = text.Length;
        //    if (endChar < stringLen)
        //        stringLen = endChar;

        //    bool foundStartLine = (lineStart == 0);
        //    m_lineCnt = 0;
        //    while ((characterIndex < stringLen) && (m_lineCnt < m_maxLines))
        //    {
        //        FillLine(ref m_lines[m_lineCnt], text, stringLen, ref characterIndex, m_maxWidth);


        //        ++m_lineCnt;
        //        if (foundStartLine == false && m_lineCnt == lineStart)
        //        {
        //            m_lineCnt = 0;
        //            foundStartLine = true;
        //        }
        //    }

        //     If the start line was not found make sure we blank out the text.
        //    if (foundStartLine == false)
        //    {
        //        m_lineCnt = 0;
        //    }
        //}

        public void FillLine(ref Line line, string text, int stringLen, ref int characterIndex, int maxWidth)
        {
            int width = 0;
            int widthAtBreak = 0;
            int glyphCnt = 0;
            int lastBreakIndex = -1;

            // Skip the white spaces at the start of the line.
            //while (characterIndex < stringLen && text[characterIndex] == ' ')
            //{
            //    ++characterIndex;
            //}

            line.m_startCharacter = characterIndex;


            do
            {
                if (characterIndex == stringLen)
                {
                    widthAtBreak = width;
                    lastBreakIndex = characterIndex;
                    break;
                }

                char character = text[characterIndex];

                if (character == ' ')
                {
                    widthAtBreak = width;
                    lastBreakIndex = characterIndex;
                }

                GlyphInfo glyphInfo = m_font.GetGlyphInfo(character);

                bool lineBreak = ((width + glyphInfo.m_dimension.X) > maxWidth) || (character == '\n');

                if (lineBreak)
                {
                    if (lastBreakIndex == -1 || m_canBreakWord)
                    {
                        lastBreakIndex = characterIndex;
                        widthAtBreak = width;
                    }
                    break;
                }

                width += glyphInfo.m_advance;
                line.m_glyphInfo[glyphCnt++] = glyphInfo;

                ++characterIndex;
            } while (true);


            line.m_glyphCnt = lastBreakIndex - line.m_startCharacter;
            line.m_width = widthAtBreak;
            characterIndex = lastBreakIndex;
        }


        public string DebugGenerateWordWrappedString()
        {
            StringBuilder stringBuilder = new StringBuilder(300);

            for(int l = 0; l < m_lineCnt; ++l)
            {
                Line line = m_lines[l];

                stringBuilder.Append('"');
                for(int g = 0; g < line.m_glyphCnt; ++g)
                {
                    GlyphInfo glyphInfo = line.m_glyphInfo[g];
                    stringBuilder.Append(glyphInfo.m_character);
                }

                stringBuilder.Append('"');
                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }
    }
}
