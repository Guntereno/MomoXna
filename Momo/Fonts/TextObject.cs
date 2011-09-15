using System;
using System.Text;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Momo.Fonts
{
    public class TextObject
    {
        // --------------------------------------------------------------------
        // -- Protected Members
        // --------------------------------------------------------------------
        protected string m_stringText = null;
        protected char[] m_charArrayText = null;

        protected TextStyle m_textStyle = null;
        protected Color[] m_colours = new Color[2] { Color.White, Color.Black };

        protected WordWrapper m_wordWrapper = null;

        protected Vector2 m_position = Vector2.Zero;
        protected Vector2 m_scale = Vector2.One;
        protected float m_width;

        protected VerticalAlignment m_verticalAlignment = VerticalAlignment.kTop;
        protected HorizontalAlignment m_horizontalAlignment = HorizontalAlignment.kLeft;



        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties       
        public TextStyle TextStyle
        {
            get { return m_textStyle; }
        }

        public Color PrimaryColour
        {
            get { return m_colours[0]; }
            set { m_colours[0] = value; }
        }

        public Color SecondaryColour
        {
            get { return m_colours[1]; }
            set { m_colours[1] = value; }
        }

        public Color[] Colours
        {
            get { return m_colours; }
            set { m_colours = value; }
        }

        public WordWrapper WordWrapper
        {
            get { return m_wordWrapper; }
        }

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public Vector2 Scale
        {
            get { return m_scale; }
            set {
                m_scale = value;
                SetWidth(m_width);
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return m_verticalAlignment; }
            set { m_verticalAlignment = value; }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return m_horizontalAlignment; }
            set { m_horizontalAlignment = value; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public TextObject(int maxChars, int maxLines)
        {
            m_wordWrapper = new WordWrapper(maxChars, maxLines);
        }

        public TextObject(TextStyle style, int width, int maxChars, int maxLines)
        {
            m_wordWrapper = new WordWrapper(maxChars, maxLines);
            m_width = width;
            m_textStyle = style;
        }

        public TextObject(string text, TextStyle style, int width, int maxChars, int maxLines)
        {
            m_wordWrapper = new WordWrapper(maxChars, maxLines);
            SetText(text, style, width);
        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void Reset()
        {
            m_stringText = null;
            m_charArrayText = null;
            m_textStyle = null;
        }


        public void SetText(string text)
        {
            m_stringText = text;
            m_charArrayText = null;

            m_wordWrapper.SetText(text, m_textStyle.Font, (int)m_width, false);
        }


        public void SetText(string text, TextStyle style, int width)
        {
            m_stringText = text;
            m_charArrayText = null;
            m_textStyle = style;

            SetWidth(width);
        }


        public void SetText(char[] text)
        {
            m_stringText = null;
            m_charArrayText = text;

            m_wordWrapper.SetText(text, m_textStyle.Font, (int)m_width, false);
        }


        public void SetText(char[] text, TextStyle style, int width)
        {
            m_stringText = null;
            m_charArrayText = text;
            m_textStyle = style;

            SetWidth(width);
        }


        public void SetWidth(float width)
        {
            m_width = width;
            float invScaleX = 1.0f / m_scale.X;
            int scaledWidth = (int)(width * invScaleX);

            if (m_stringText != null)
                m_wordWrapper.SetText(m_stringText, m_textStyle.Font, scaledWidth, false);
            else
                m_wordWrapper.SetText(m_charArrayText, m_textStyle.Font, scaledWidth, false);
        }


        public float GetYInset()
        {
            float inset = 0.0f;

            float maxTextHeight = m_textStyle.Font.m_typeface.m_lineHeight * m_wordWrapper.MaxLines;
            float height = m_textStyle.Font.m_typeface.m_lineHeight * m_wordWrapper.LineCount;

            if (m_verticalAlignment == VerticalAlignment.kBottom)
                inset = (float)Math.Round((maxTextHeight - (height * m_scale.Y)));
            else if (m_verticalAlignment == VerticalAlignment.kMiddle)
                inset = (float)Math.Round((maxTextHeight - (height * m_scale.Y)) * 0.5f);

            return inset;
        }

        public float GetXInset(int line)
        {
            float inset = 0.0f;

            if (m_horizontalAlignment == HorizontalAlignment.kRight)
                inset = (float)Math.Round((m_width - (m_wordWrapper.Lines[line].m_width * m_scale.X)));
            else if (m_horizontalAlignment == HorizontalAlignment.kMiddle)
                inset = (float)Math.Round((m_width - (m_wordWrapper.Lines[line].m_width * m_scale.X)) * 0.5f);

            return inset;
        }


        // --------------------------------------------------------------------
        // -- Private Methods
        // --------------------------------------------------------------------
    }
}
