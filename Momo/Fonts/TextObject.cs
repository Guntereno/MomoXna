using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Fonts
{
    public class TextObject
    {
        // --------------------------------------------------------------------
        // -- Protected Members
        // --------------------------------------------------------------------
        private string m_text = null;

        protected Font m_font = null;
        protected Color[] m_colour = new Color[2] { Color.White, Color.Black };

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
        public string Text
        {
            get { return m_text; }
            set { SetText(value); }
        }
        
        public Font Font
        {
            get { return m_font; }
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

        public Color Colour
        {
            get { return m_colour[0]; }
            set { m_colour[0] = value; }
        }

        public Color OutlineColour
        {
            get { return m_colour[1]; }
            set { m_colour[1] = value; }
        }

        public Color[] Colours
        {
            get { return m_colour; }
            set { m_colour = value; }
        }

        #endregion


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public TextObject(string text, Font font, int width, int maxCharsPerLine, int maxLines)
        {
            m_wordWrapper = new WordWrapper(maxCharsPerLine, maxLines);
            SetText(text, font, width);
        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void SetText(string text)
        {
            m_text = text;
            m_wordWrapper.SetText(text);
        }

        public void SetText(string text, Font font, int width)
        {
            m_text = text;
            m_font = font;
            SetWidth((float)width);
        }

        public void SetWidth(float width)
        {
            float invScaleX = 1.0f / m_scale.X;
            m_width = width;
            m_wordWrapper.SetText(m_text, m_font, (int)(width * invScaleX));
        }

        public float GetYInset()
        {
            float inset = 0.0f;

            float maxTextHeight = m_font.m_typeface.m_lineHeight * m_wordWrapper.MaxLines;
            float height = m_font.m_typeface.m_lineHeight * m_wordWrapper.LineCount;
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
        // -- Protected Methods
        // --------------------------------------------------------------------
    }
}
