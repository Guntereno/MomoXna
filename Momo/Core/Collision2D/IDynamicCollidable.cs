using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Collision2D
{
	public interface IDynamicCollidable
	{
		// --------------------------------------------------------------------
		// -- Internal Members
		// --------------------------------------------------------------------
		void SetPosition(Vector2 position);
		Vector2 GetPosition();


		void SetVelocity(Vector2 velocity);
		Vector2 GetVelocity();


		void SetForce(Vector2 force);
		Vector2 GetForce();
		Vector2 GetLastFrameAcceleration();


		void SetMass(float mass);
		float GetMass();
		float GetInverseMass();



		//float Rotation
		//{
		//	get;
		//	set;
		//}

		//float Scale
		//{
		//	get;
		//	set;
		//}

		//bool CanSleep
		//{
		//	get;
		//	set;
		//}

		//bool IsAwake
		//{
		//	get;
		//}

		//float AngularVelocity
		//{
		//	get;
		//	set;
		//}


		//void SetAwake(bool isAwake);
	}
}
