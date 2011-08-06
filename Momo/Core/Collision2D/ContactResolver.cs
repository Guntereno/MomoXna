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
		private const float kPenetrationEpsilon = 0.0015f;				// Ignore contacts with a peneration smaller than.
		private const float kSeperatingVelocityEpsilon = -0.02f;		// Ignore contacts with a seperating velocity larger than.

		private const int kVelocityResolveContactMultiplier = 2;	   // What factor of the number of contacts do we resolve
																		// the velocities.
		private const int kPenetrationResolveContactMulitplier = 3;	 // What factor of the number of contacts do we resolve
																		// the penerations.



		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public void ResolveContacts(float dt, ContactList contactList)
		{
			int contactListCnt = contactList.GetContactCount();
			Contact[] contacts = contactList.GetContactList();


			for (int i = 0; i < contactListCnt; ++i)
			{
				contacts[i].calculateInternals();
			}


			int iterations = contactListCnt * kVelocityResolveContactMultiplier;
			for (int l = 0; l < iterations; ++l)
			{
				float max = kSeperatingVelocityEpsilon;
				int maxIndex = contactListCnt;

				for (int i = 0; i < contactListCnt; ++i)
				{
					if (contacts[i].m_penetration > 0.0f)
					{
						if (contacts[i].m_seperatingVelocity < max)
						{
							max = contacts[i].m_seperatingVelocity;
							maxIndex = i;
						}
					}
				}

				// Do we have anything worth resolving?
				if (maxIndex == contactListCnt)
					break;

				contacts[maxIndex].matchAwakeState();

				// Resolve the velocity
				contacts[maxIndex].resolveVelocity(dt);
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
		}
	}
}
