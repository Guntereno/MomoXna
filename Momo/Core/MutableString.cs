﻿using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;



namespace Momo.Core
{
    public class MutableString
    {
        private static readonly char[] kDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private char[] m_string = null;
        private int m_length = 0;



        public MutableString(int maxLength)
        {
            m_string = new char[maxLength];
        }


        public int GetLength()
        {
            return m_length;
        }

        public void Clear()
        {
            m_length = 0;
            EndAppend();
        }

        public void EndAppend()
        {
            m_string[m_length] = '\0';
        }

        public void Set(string value)
        {
            m_length = 0;
            Append(value);
            EndAppend();
        }

        public void Set(char[] value)
        {
            m_length = 0;
            Append(value);
            EndAppend();
        }

        public void Set(MutableString value)
        {
            m_length = 0;
            Append(value.GetCharacterArray());
            EndAppend();
        }

        public void Set(int value)
        {
            m_length = 0;
            Append(value);
            EndAppend();
        }

        public void Set(float value, int decimalPlaces)
        {
            m_length = 0;
            Append(value, decimalPlaces);
            EndAppend();
        }

        public void Append(string value)
        {
            int valueLength = value.Length;
            value.CopyTo(0, m_string, m_length, valueLength);
            m_length += valueLength;
        }

        public void Append(MutableString value)
        {
            int valueLength = value.GetLength();
            Array.Copy(value.GetCharacterArray(), 0, m_string, m_length, valueLength);
            m_length += valueLength;
        }

        public void Append(char value)
        {
            m_string[m_length++] = value;
        }

        public void Append(char[] value)
        {
            value.CopyTo(m_string, m_length);
            m_length += value.Length;
        }

        public void Append(bool value)
        {
            if (value)
                Append("true");
            else
                Append("false");
        }

        public void AppendLine()
        {
            Append('\n');
        }

        public void AppendLine(string value)
        {
            Append(value);
            AppendLine();
        }

        public void Insert(int index, string value)
        {
            int valueLength = value.Length;
            m_length = (index + valueLength);
            value.CopyTo(0, m_string, index, valueLength);
        }


        public void Append(int value)
        {
            Append(value, 0);
        }


        public void Append(int value, int padSize)
        {
            if (value < 0)
            {
                Append('-');
                uint uintValue = uint.MaxValue - ((uint)value) + 1; //< This is to deal with Int32.MinValue
                Append(uintValue, padSize);
            }
            else
            {
                Append((uint)value, padSize);
            }
        }


        public void Append(uint value)
        {
            Append(value, 0);
        }


        public void Append(uint value, int padSize)
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

            if (padSize > length)
                length = padSize;

            m_length = (m_length + length);
            int strpos = m_length;

            // We're writing backwards, one character at a time.
            while (length > 0)
            {
                strpos--;

                // Lookup from static char array, to cover hex values too
                char digit = kDigits[value % 10];
                m_string[strpos] = digit;

                value /= 10;
                length--;
            }
        }

        
        public void Append(float floatVal, int decimalPlaces)
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
                remainder = remainder * (float)Math.Pow(10, decimalPlaces);

                // Round up. It's guaranteed to be a positive number, so no extra work required here.
                remainder += 0.5f;

                // All done, print that as an int!
                Append((uint)remainder, decimalPlaces);
            }
        }


        public char[] GetCharacterArray()
        {
            return m_string;
        }
    }
}
