using System;
using System.Collections.Generic;



namespace Momo.Core
{
    public interface INamed
    {
        void SetName(MutableString name);
        void SetName(string name);

        MutableString GetName();
        int GetNameHash();
    }


    // Basic implementation of INamed
    public struct Name : INamed
    {
        private MutableString m_name;
        private int m_nameHash;


        public Name(int maxNameLength)
        {
            m_name = new MutableString(maxNameLength);
            m_nameHash = 0;
        }


        // --------------------------------------------------------------------
        // -- INamed interface implementation
        // --------------------------------------------------------------------
        public void SetName(MutableString name)
        {
            m_name.Set(name);
            m_nameHash = Hashing.GenerateHash(m_name.GetCharacterArray(), m_name.GetLength());
        }

        public void SetName(string name)
        {
            m_name.Set(name);
            m_nameHash = Hashing.GenerateHash(m_name.GetCharacterArray(), m_name.GetLength());
        }

        public MutableString GetName()
        {
            return m_name;
        }

        public int GetNameHash()
        {
            return m_nameHash;
        }
    }

}
