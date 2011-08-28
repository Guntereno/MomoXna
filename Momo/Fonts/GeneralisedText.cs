using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fonts
{
    public interface GeneralisedText
    {
        char GetCharacter(int index);
        bool IsEnd(int index);
    }


    public struct GeneralisedStringText : GeneralisedText
    {
        private string m_string;

        public GeneralisedStringText(string str)
        {
            m_string = str;
        }

        public char GetCharacter(int index)
        {
            return m_string[index];
        }

        public bool IsEnd(int index)
        {
            return (index >= m_string.Length);
        }
    }


    public struct GeneralisedCharArrayText : GeneralisedText
    {
        private char[] m_string;


        public GeneralisedCharArrayText(char[] str)
        {
            m_string = str;
        }

        public char GetCharacter(int index)
        {
            return m_string[index];
        }

        public bool IsEnd(int index)
        {
            return (m_string[index] == '\0');
        }
    }
}
