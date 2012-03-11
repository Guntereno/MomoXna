using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Momo.Core.Collision2D
{
    public class ContactResolver
    {
        // --------------------------------------------------------------------
        // -- Constants
        // --------------------------------------------------------------------
        private const float kPenetrationEpsilon = 0.0015f;                // Ignore contacts with a peneration smaller than.
        private const float kSeperatingVelocityEpsilon = -0.02f;        // Ignore contacts with a seperating velocity larger than.

        private const int kVelocityResolveContactMultiplier = 2;       // What factor of the number of contacts do we resolve
                                                                        // the velocities.
        private const int kPenetrationResolveContactMulitplier = 3;     // What factor of the number of contacts do we resolve
                                                                        // the penerations.

        // A subset of the passed in contact list
        static readonly int kMaxNonPenerationContacts = 4000;
        private Contact[] m_nonPenerationContacts = new Contact[kMaxNonPenerationContacts];


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void ResolveContacts(float dt, ContactList contactList)
        {
            int contactListCnt = contactList.GetContactCount();
            Contact[] contacts = contactList.GetContactList();

            int nonPenerationContactCount = 0;


            for (int i = 0; i < contactListCnt; ++i)
            {
                Contact contact = contacts[i];
                contact.calculateInternals();

                if(contact.m_penetration > 0.0f)
                    m_nonPenerationContacts[nonPenerationContactCount++] = contacts[i];
            }


            int iterations = contactListCnt * kVelocityResolveContactMultiplier;
            for (int l = 0; l < iterations; ++l)
            {
                float max = kSeperatingVelocityEpsilon;
                int maxIndex = contactListCnt;

                for (int i = 0; i < nonPenerationContactCount; ++i)
                {
                    if (m_nonPenerationContacts[i].m_seperatingVelocity < max)
                    {
                        // We are using a pre filtered list now.
                        //if (contacts[i].m_penetration > 0.0f)
                        //{
                        max = m_nonPenerationContacts[i].m_seperatingVelocity;
                        maxIndex = i;
                        //}
                    }
                }

                // Do we have anything worth resolving?
                if (maxIndex == contactListCnt)
                    break;

                m_nonPenerationContacts[maxIndex].matchAwakeState();

                // Resolve the velocity
                m_nonPenerationContacts[maxIndex].resolveVelocity(dt);


                //for (int i = 0; i < contactListCnt; ++i)
                //{
                //    if (contacts[i].m_seperatingVelocity < max)
                //    {
                //        if (contacts[i].m_penetration > 0.0f)
                //        {
                //            max = contacts[i].m_seperatingVelocity;
                //            maxIndex = i;
                //        }
                //    }
                //}

                //// Do we have anything worth resolving?
                //if (maxIndex == contactListCnt)
                //    break;

                //contacts[maxIndex].matchAwakeState();

                //// Resolve the velocity
                //contacts[maxIndex].resolveVelocity(dt);
            }


            iterations = contactListCnt * kPenetrationResolveContactMulitplier;
            for (int l = 0; l < iterations; ++l)
            {
                float max = kPenetrationEpsilon;
                int maxIndex = contactListCnt;

                for (int i = 0; i < contactListCnt; ++i)
                {
                    if (contacts[i].m_penetration > max)
                    {
                        max = contacts[i].m_penetration;
                        maxIndex = i;
                    }
                }

                // Do we have anything worth resolving?
                if (maxIndex == contactListCnt)
                    break;


                contacts[maxIndex].matchAwakeState();

                // Resolve the contact
                contacts[maxIndex].resolveInterpeneration(dt);


                // Update the penetration values now that this object has moved (during the resolve).
                for (int i = 0; i < contacts[maxIndex].m_pairedContactCnt; ++i)
                {
                    Contact pairedContact = contacts[maxIndex].m_pairedContacts[i];
                    if (pairedContact.m_object1 == contacts[maxIndex].m_object1)
                    {
                        pairedContact.m_penetration -= Vector2.Dot(contacts[maxIndex].m_resolutionMovement1, pairedContact.m_contactNormal);
                    }
                    else if (pairedContact.m_object1 == contacts[maxIndex].m_object2)
                    {
                        pairedContact.m_penetration -= Vector2.Dot(contacts[maxIndex].m_resolutionMovement2, pairedContact.m_contactNormal);
                    }
                    if (pairedContact.m_object2 != null)
                    {
                        if (pairedContact.m_object2 == contacts[maxIndex].m_object1)
                        {
                            pairedContact.m_penetration += Vector2.Dot(contacts[maxIndex].m_resolutionMovement1, pairedContact.m_contactNormal);
                        }
                        else if (pairedContact.m_object2 == contacts[maxIndex].m_object2)
                        {
                            pairedContact.m_penetration += Vector2.Dot(contacts[maxIndex].m_resolutionMovement2, pairedContact.m_contactNormal);
                        }
                    }
                }
            }


            for(int i = 0; i < nonPenerationContactCount; ++i)
            {
                m_nonPenerationContacts[i] = null;
            }
        }
    }
}
