using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Collision2D
{
    public class ContactIsland
    {
        internal Contact m_firstContact = null;
        internal Contact m_lastContact = null;


        public void Reset()
        {
            m_firstContact = null;
            m_lastContact = null;
        }


        public void AddContact(Contact contact)
        {
            if (m_lastContact != null)
            {
                m_lastContact.m_nextIslandContact = contact;
                m_lastContact = contact;
            }
            else
            {
                m_firstContact = contact;
                m_lastContact = contact;
            }
        }


        public void MergeIslandInto(ContactIsland collisionIsland)
        {
            m_lastContact.m_nextIslandContact = collisionIsland.m_firstContact;
            m_lastContact = collisionIsland.m_lastContact;

            collisionIsland.m_lastContact = null;
            collisionIsland.m_firstContact = null;
        }
    }
}
