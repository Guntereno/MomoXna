using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WorldManager;


namespace TestGame
{
	public class TestWorld : World
	{
		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public static World WorldCreator()
		{
			return new TestWorld();
		}


		public override void Load()
		{
		}


		public override void Enter()
		{
		}


		public override void Update(float dt)
		{
		}


		public override void Exit()
		{
		}


		public override void Flush()
		{
		}


		public override void PostRender()
		{
		}


		public override void Render()
		{
		}


		public override void PreRender()
		{
		}


		public override void DebugRender()
		{
		}
	}
}
