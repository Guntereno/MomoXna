using System;

using Microsoft.Xna.Framework;


namespace Momo.Fonts
{
    public enum TextSecondaryDrawTechnique
    {
        kNone,
        kOutline,
        kDropshadow,
    }


    public class TextStyle
    {
        protected Font m_font = null;
        protected TextSecondaryDrawTechnique m_secondaryDrawTechnique = TextSecondaryDrawTechnique.kNone;

        protected Vector2 m_dropShadowOffset = new Vector2(1.5f, 1.5f);



        public TextStyle(Font font)
        {
            m_font = font;
            m_secondaryDrawTechnique = TextSecondaryDrawTechnique.kNone;
        }


        public TextStyle(Font font, TextSecondaryDrawTechnique secondaryDraw)
        {
            m_font = font;
            m_secondaryDrawTechnique = secondaryDraw;
        }


        public void SetDrawDropShadowOffset(Vector2 dropShadowOffset)
        {
            m_dropShadowOffset = dropShadowOffset;
        }


        #region Properties
        public Font Font
        {
            get { return m_font; }
        }

        public TextSecondaryDrawTechnique SecondaryDrawTechnique
        {
            get { return m_secondaryDrawTechnique; }
            set { m_secondaryDrawTechnique = value; }
        }

        public Vector2 DropShadowOffset
        {
            get { return m_dropShadowOffset; }
        }
        #endregion
    }
}
