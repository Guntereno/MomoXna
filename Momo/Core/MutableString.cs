using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core
{
    public class MutableString
    {
        private static readonly char[] kDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };


        private StringBuilder m_stringBuilder = null;
        private char[] m_internalString = null;


        public MutableString(int maxLength)
        {
            m_stringBuilder = new StringBuilder(maxLength, maxLength);

            m_internalString = (char[])m_stringBuilder.GetType().GetField(
                                        "m_ChunkChars",
                                        System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Instance).GetValue(m_stringBuilder);
        }


        public void Clear()
        {
            m_stringBuilder.Length = 0;
        }

        public void EndAppend()
        {
            m_stringBuilder.Append('\0');
        }

        public void Append(string value)
        {
            m_stringBuilder.Append(value);
        }

        public void Append(char value)
        {
            m_stringBuilder.Append(value);
        }

        public void Append(char[] value)
        {
            m_stringBuilder.Append(value);
        }

        public void Append(bool value)
        {
            m_stringBuilder.Append(value);
        }

        public void AppendLine()
        {
            m_stringBuilder.AppendLine();
        }

        public void AppendLine(string value)
        {
            m_stringBuilder.AppendLine(value);
        }

        public void Insert(int index, string value)
        {
            m_stringBuilder.Insert(index, value);
        }


        public void Append(int value)
        {
            if (value < 0)
            {
                m_stringBuilder.Append('-');
                uint uintValue = uint.MaxValue - ((uint)value) + 1; //< This is to deal with Int32.MinValue
                Append(uintValue);
            }
            else
            {
                Append((uint)value);
            }
        }


        public void Append(uint value)
        {
            // Calculate length of integer when written out
            int length = 0;
            int lengthCalc = (int)value;

            do
            {
                lengthCalc /= 10;
                length++;
            }
            while (lengthCalc > 0);


            int strpos = length;
            m_stringBuilder.Length = length;

            // We're writing backwards, one character at a time.
            while (length > 0)
            {
                strpos--;

                // Lookup from static char array, to cover hex values too
                char digit = kDigits[value % 10];
                m_stringBuilder[strpos] = digit;

                value /= 10;
                length--;
            }
        }

        
        public void Append(float floatVal, uint decimalPlaces)
        {
            if (decimalPlaces == 0)
            {
                int intVal;
                if (floatVal >= 0.0f)
                    intVal = (int)(floatVal + 0.5f);
                else
                    intVal = (int)(floatVal - 0.5f);

                Append(intVal);
            }
            else
            {
                int intPart = (int)floatVal;

                // First part is easy, just cast to an integer
                Append(intPart);

                Append('.');

                // Work out remainder we need to print after the d.p.
                float remainder = Math.Abs(floatVal - intPart);

                // Multiply up to become an int that we can print
                do
                {
                    remainder *= 10;
                    decimalPlaces--;
                }
                while (decimalPlaces > 0);

                // Round up. It's guaranteed to be a positive number, so no extra work required here.
                remainder += 0.5f;

                // All done, print that as an int!
                Append((uint)remainder);
            }
        }


        public char[] GetCharacterArray()
        {
            return m_internalString;
        }
    }
}
