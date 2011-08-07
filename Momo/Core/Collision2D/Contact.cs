using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Momo.Core.Collision2D
{
	public class Contact
	{
		// --------------------------------------------------------------------
		// -- Public Members
		// --------------------------------------------------------------------
		public IDynamicCollidable m_object1 = null;
		public IDynamicCollidable m_object2 = null;

		public float m_friction;
		public float m_restitution;
		public float m_penetration;
		public float m_seperatingVelocity;

		public Vector2 m_contactNormal;

		static int MAX_PAIRED_CONTACTS = 10;
		public Contact [] m_pairedContacts = new Contact[MAX_PAIRED_CONTACTS];
		public int m_pairedContactCnt = 0;


		// --------------------------------------------------------------------
		// -- Internal Members
		// --------------------------------------------------------------------
		internal float m_totalInverseMass;
		internal Vector2 m_resolutionMovement1;
		internal Vector2 m_resolutionMovement2;
		internal Vector2 m_relativeVelocity;



		// --------------------------------------------------------------------
		// -- Internal Members
		// --------------------------------------------------------------------
		internal void addPairedContact(Contact contact)
		{
			if (m_pairedContactCnt < MAX_PAIRED_CONTACTS)
			{
				m_pairedContacts[m_pairedContactCnt] = contact;
				++m_pairedContactCnt;
			}
		}


		internal void Reset()
		{
			m_object1 = null;
			m_object2 = null;

			for (int i = 0; i < m_pairedContactCnt; ++i)
			{
				m_pairedContacts[i] = null;
			}

			m_pairedContactCnt = 0;
		}


		internal void matchAwakeState()
		{
			//// For now collisions with the world will not wake it up, however, at some
			//// point this will probably have to change to deal with moving world bits.
			//if (m_object2 == null)
			//	return;

			//// If either objects are awake then...
			//if (m_object1.IsAwake ^ m_object2.IsAwake)
			//{
			//	if (m_object1.IsAwake)
			//		m_object2.SetAwake(true);
			//	else
			//		m_object1.SetAwake(true);
			//}
		}


		internal void calculateSeperatingVelocity()
		{
			if (m_penetration > 0.0f)
			{
				m_relativeVelocity = m_object1.GetVelocity();

				if (m_object2 != null)
					m_relativeVelocity -= m_object2.GetVelocity();

				m_seperatingVelocity = Vector2.Dot(m_relativeVelocity, m_contactNormal);
			}
		}


		internal void calculateInternals()
		{
			m_totalInverseMass = m_object1.GetInverseMass();
			
			if (m_object2 != null)
				m_totalInverseMass += m_object2.GetInverseMass();
			
			calculateSeperatingVelocity();
		}


		internal void resolveVelocity(float dt)
		{
			// Dont allow resolve velocity to be called if its seperating velocity is + (ie moving away from each other).
			//if (seperatingVelocity > 0.0f)
			//	return;

			float newSeperatingVelocity = -m_seperatingVelocity * m_restitution;


			Vector2 accCausedVelocity = m_object1.GetLastFrameAcceleration();
			if (m_object2 != null)
				accCausedVelocity -= m_object2.GetLastFrameAcceleration();

			float accCausedSeperatingVelocity = Vector2.Dot(accCausedVelocity, m_contactNormal) * dt;

			if (accCausedSeperatingVelocity < 0.0f)
			{
				newSeperatingVelocity += m_restitution * accCausedSeperatingVelocity;

				if (newSeperatingVelocity < 0.0f)
					newSeperatingVelocity = 0.0f;
			}

			float deltaVelocity = newSeperatingVelocity - m_seperatingVelocity;

			// I dont intend to have 2 infinite mass objects colliding, or negative massed objects
			// for that matter!
			//if (m_totalInverseMass <= 0.0f)
			//	return;


			float impulse = deltaVelocity / m_totalInverseMass;
			Vector2 impulsePerIMass = m_contactNormal * impulse;


			if (m_object2 == null)
			{
				Vector2 planarVelocity = (m_relativeVelocity - (m_contactNormal * Vector2.Dot(m_relativeVelocity, m_contactNormal)));
				float planarVelocityLength = planarVelocity.LengthSquared();

				if (planarVelocityLength > 0.0f)
				{
					planarVelocityLength = (float)Math.Sqrt(planarVelocityLength);
					float frictionMag = m_friction * 5.0f * dt;
					if (frictionMag > planarVelocityLength)
						frictionMag = planarVelocityLength;

					Vector2 friction = (planarVelocity / planarVelocityLength) * frictionMag * m_object1.GetMass();

					impulsePerIMass -= friction;
				}
			}


			m_object1.SetVelocity( m_object1.GetVelocity() + (impulsePerIMass * m_object1.GetInverseMass()));
			if (m_object2 != null)
				m_object2.SetVelocity(m_object2.GetVelocity() + (impulsePerIMass * -m_object2.GetInverseMass()));



			calculateSeperatingVelocity();
			for (int i = 0; i < m_pairedContactCnt; ++i)
			{
				m_pairedContacts[i].calculateSeperatingVelocity();
			}
		}


		internal void resolveInterpeneration(float dt)
		{
			// Make sure when selecting the contact, check the peneration then.
			//if (m_penetration <= 0.0f)
			//	return;
			
			// I dont intend on having infinite masses hitting each other.
			//if (totalInverseMass <= 0.0f)
			//	return;

			float penerationPerIMass = (m_penetration / m_totalInverseMass);
			Vector2 movePerIMass = m_contactNormal * penerationPerIMass;

			m_resolutionMovement1 = movePerIMass * m_object1.GetInverseMass();
			m_object1.SetPosition(m_object1.GetPosition() + m_resolutionMovement1);
			m_penetration -= penerationPerIMass * m_object1.GetInverseMass();

            m_object1.OnCollisionEvent(ref m_object2);

			if (m_object2 != null)
			{
				m_resolutionMovement2 = movePerIMass * -m_object2.GetInverseMass();
				m_object2.SetPosition(m_object2.GetPosition() + m_resolutionMovement2);
				m_penetration += penerationPerIMass * -m_object2.GetInverseMass();

                m_object2.OnCollisionEvent(ref m_object1);
			}
			else
			{
				m_resolutionMovement2 = Vector2.Zero;
			}
		}

	}
}
