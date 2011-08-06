using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Nodes
{
	// ------------------------------------------------------------------------
	// -- Class Declaration
	// ------------------------------------------------------------------------
	public class Node3d
	{
		// --------------------------------------------------------------------
		// -- Protected Members
		// --------------------------------------------------------------------
		protected Node3d m_parent = null;
		protected List<Node3d> m_childList = null;

		protected Matrix m_matrix = Matrix.Identity;


		// --------------------------------------------------------------------
		// -- Private Members
		// --------------------------------------------------------------------
		private String m_name;


		// --------------------------------------------------------------------
		// -- Public Properties
		// --------------------------------------------------------------------
		#region Properties
		public String Name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		public Node3d Parent
		{
			get { return m_parent; }
			set { m_parent = value; }
		}

		public List<Node3d> Children
		{
			get { return m_childList; }
			set { m_childList = value; }
		}

		public Matrix Matrix
		{
			get{ return m_matrix; }
			set
			{
				m_matrix = value;
			}
		}

		public Vector3 LocalTranslation
		{
			get { return m_matrix.Translation; }
			set { m_matrix.Translation = value; }
		}
		#endregion


		// --------------------------------------------------------------------
		// -- Constructor/Deconstructor
		// --------------------------------------------------------------------
		public Node3d(string name)
		{
			m_name = name;
		}


		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public void AddChild(Node3d node)
		{
			// Init the child list on the first use.
			if (m_childList == null)
				m_childList = new List<Node3d>(2);

			node.Parent = this;
			m_childList.Add(node);
		}


		public bool RemoveChild(Node3d node)
		{
			// No childlist, so no removing occuring.
			if (m_childList == null)
				return false;

			node.Parent = null;
			return m_childList.Remove(node);
		}


		public void GenerateGlobalMatrix( ref Matrix outGlobalMatrix )
		{
			if (m_parent == null)
			{
				outGlobalMatrix = m_matrix;
				return;
			}

			outGlobalMatrix = outGlobalMatrix * m_matrix;
		}


		public void SetGlobalMatrix(Matrix globalMatrix)
		{
			Vector3 scale = new Vector3(globalMatrix.Left.Length(), globalMatrix.Up.Length(), globalMatrix.Forward.Length());

			if (m_parent == null)
			{
				m_matrix = globalMatrix;
				return;
			}

			// Update the parents, then use it to calculate yours.
			Matrix parentGlobalMatrix = new Matrix();
			m_parent.GenerateGlobalMatrix(ref parentGlobalMatrix);

			m_matrix = Matrix.Invert(globalMatrix) * parentGlobalMatrix;
		}

	}
}
