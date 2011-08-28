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
        public int m_endGlyphIdx;
        public int m_width;
    }


    public class WordWrapper
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private int m_maxWidth;

        private readonly int m_maxChars = 0;
        private readonly int m_maxLines = 0;

        private Line[] m_lines = null;
        private int m_lineCnt = 0;

        public GlyphInfo[] m_glyphInfo;
        public int m_glyphInfoCnt;



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
            get { return m_maxChars; }
        }

        public int MaxLines
        {
            get { return m_maxLines; }
        }
        
        public GlyphInfo[] GlyphInfo
        {
            get { return m_glyphInfo; }
        }

        public int GlyphInfoCount
        {
            get { return m_glyphInfoCnt; }
        }

        public Line[] Lines
        {
            get { return m_lines; }
        }

        public int LineCount
        {
            get { return m_lineCnt; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public WordWrapper(int maxChars, int maxLines)
        {
            m_maxChars = maxChars;
            m_maxLines = maxLines;

            m_lines = new Line[maxLines];

            m_glyphInfo = new GlyphInfo[maxChars];
            m_glyphInfoCnt = 0;
        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void SetText(string text, Font font, int width, bool canBreakWord)
        {
            GeneralisedStringText str = new GeneralisedStringText(text);
            SetText(str, font, width, canBreakWord);
        }


        public void SetText(char[] text, Font font, int width, bool canBreakWord)
        {
            GeneralisedCharArrayText str = new GeneralisedCharArrayText(text);
            SetText(str, font, width, canBreakWord);
        }


        public void SetText(GeneralisedText text, Font font, int width, bool canBreakWord)
        {
            Clear();

            m_maxWidth = width;

            int characterIndex = 0;

            while (!text.IsEnd(characterIndex) && (m_lineCnt < m_maxLines))
            {
                FillLine(ref m_lines[m_lineCnt], text, font, ref characterIndex, m_maxWidth, canBreakWord);
                ++m_lineCnt;
            }
        }


        public void Clear()
        {
            m_maxWidth = 0;
            m_lineCnt = 0;

            // Blank out the array so that it does not hang on to dangling pointers.
            for (int i = 0; i < m_glyphInfoCnt; ++i)
                m_glyphInfo[i] = null;

            m_glyphInfoCnt = 0;
        }


        public void FillLine(ref Line line, GeneralisedText text, Font font, ref int characterIndex, int maxWidth, bool canBreakWord)
        {
            int width = 0;
            int widthAtBreak = 0;
            int lastBreakIndex = -1;
            GlyphInfo lastGlyphInfo = null;


            // Skip the white spaces at the start of the line.
            char c = ' ';
            while (!text.IsEnd(characterIndex))
            {
                c = text.GetCharacter(characterIndex);
                if ((c != ' ' && c != '\n'))
                    break;

                ++characterIndex;
            }



            do
            {
                if (text.IsEnd(characterIndex))
                {
                    widthAtBreak = width;
                    lastBreakIndex = characterIndex;
                    break;
                }

                c = text.GetCharacter(characterIndex);

                if (c == ' ')
                {
                    widthAtBreak = width;
                    lastBreakIndex = characterIndex;
                }

                GlyphInfo glyphInfo = font.GetGlyphInfo(c);
                int kerning = 0;

                if (lastGlyphInfo != null)
                    kerning = lastGlyphInfo.GetKerningInfo(glyphInfo.m_character);


                bool lineBreak = ((width + glyphInfo.m_dimension.X + kerning) > maxWidth) || (c == '\n');

                if (lineBreak)
                {
                    if (lastBreakIndex == -1 || canBreakWord)
                    {
                        lastBreakIndex = characterIndex;
                        widthAtBreak = width;
                    }
                    lastGlyphInfo = null;
                    break;
                }

                width += glyphInfo.m_advance + kerning;


                m_glyphInfo[m_glyphInfoCnt++] = glyphInfo;

                lastGlyphInfo = glyphInfo;
                ++characterIndex;
            } while (true);


            line.m_width = widthAtBreak;
            line.m_endGlyphIdx = m_glyphInfoCnt - 1;

            characterIndex = lastBreakIndex;
        }
    }
}
