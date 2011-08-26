using System.Collections.Generic;

using Microsoft.Xna.Framework;



namespace Momo.Core.Collision2D
{
    public class ContactList
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Contact[] m_contacts = null;

        private int m_contactCapacity = 0;
        private int m_contactCnt = 0;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public Contact[] GetContactList()
        {
            return m_contacts;
        }

        public int GetContactCapacity()
        {
            return m_contactCapacity;
        }

        public int GetContactCount()
        {
            return m_contactCnt;
        }


        public ContactList(int contactCapacity)
        {
            m_contactCapacity = contactCapacity;

            m_contacts = new Contact[m_contactCapacity];
            for (int i = 0; i < m_contactCapacity; i++)
            {
                m_contacts[i] = new Contact();
            }
        }


        public void Reset()
        {
            // Reset the references so the garbage collector knows they are free.
            for (int i = 0; i < m_contactCnt; ++i)
            {
                m_contacts[i].Reset();
            }

            m_contactCnt = 0;
        }


        public void StartAddingContacts()
        {
            Reset();
        }


        public Contact GetContact(IDynamicCollidable collidable1, IDynamicCollidable collidable2)
        {
            for (int i = 0; i < m_contactCnt; ++i)
            {
                Contact searchContact = m_contacts[i];
                if (searchContact.m_object1 == collidable1 && searchContact.m_object2 == collidable2)
                    return searchContact;
            }

            return null;
        }


        public void AddContact(IDynamicCollidable collidable1, IDynamicCollidable collidable2, Vector2 normal, float penetration, float friction, float restitution)
        {
            Contact contact = m_contacts[m_contactCnt];
            contact.m_object1 = collidable1;
            contact.m_object2 = collidable2;
            contact.m_penetration = penetration;
            contact.m_friction = friction;
            contact.m_contactNormal = normal;
            contact.m_restitution = restitution;

            ++m_contactCnt;
        }


        public void EndAddingContacts()
        {
            for (int i = 0; i < m_contactCnt; ++i)
            {
                Contact contact1 = m_contacts[i];

                for (int j = i + 1; j < m_contactCnt; ++j)
                {
                    Contact contact2 = m_contacts[j];
                    if (contact1.m_object2 != null)
                    {
                        if (contact1.m_object1 == contact2.m_object1 || contact1.m_object1 == contact2.m_object2 ||
                            contact1.m_object2 == contact2.m_object1 || contact1.m_object2 == contact2.m_object2)
                        {
                            contact1.addPairedContact(contact2);
                            contact2.addPairedContact(contact1);
                        }
                    }
                    else
                    {
                        if (contact1.m_object1 == contact2.m_object1 || contact1.m_object1 == contact2.m_object2)
                        {
                            contact1.addPairedContact(contact2);
                            contact2.addPairedContact(contact1);
                        }
                    }

                }
            }
        }
    }
}
