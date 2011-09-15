using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Nodes.Cameras
{
    // ------------------------------------------------------------------------
    // -- Class Declaration
    // ------------------------------------------------------------------------
    public abstract class CameraNode : Node3d
    {
        // --------------------------------------------------------------------
        // -- Protected Members
        // --------------------------------------------------------------------
        protected bool m_projectionMatrixNeedsUpdate = true;

        protected Matrix m_globalMatrix = Matrix.Identity;
        protected Matrix m_unscaledGlobalMatrix = Matrix.Identity;
        protected Matrix m_viewMatrix = new Matrix();
        protected Matrix m_projectionMatrix = new Matrix();

        protected Matrix m_viewProjectionMatrix = new Matrix();
        protected Matrix m_inverseViewProjectionMatrix = new Matrix();

        protected Matrix m_rotationMatrix = new Matrix();
        protected Matrix m_inverseRotationMatrix = new Matrix();

        protected float m_nearClip = 0.1f;
        protected float m_farClip = 10000.0f;

        protected BoundingFrustum m_frustum = new BoundingFrustum( Matrix.Identity );


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public float NearClip
        {
            get { return m_nearClip; }
            set
            {
                m_nearClip = value;
                m_projectionMatrixNeedsUpdate = true;
            }
        }

        public float FarClip
        {
            get { return m_farClip; }
            set
            {
                m_farClip = value;
                m_projectionMatrixNeedsUpdate = true;
            }
        }

        public bool ProjectionMatrixNeedsUpdate
        {
            get { return m_projectionMatrixNeedsUpdate; }
        }

        public Matrix ViewMatrix
        {
            get { return m_viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return m_projectionMatrix; }
        }

        public Matrix ViewProjectionMatrix
        {
            get { return m_viewProjectionMatrix; }
        }

        public Matrix InverseViewProjectionMatrix
        {
            get { return m_inverseViewProjectionMatrix; }
        }

        public BoundingFrustum Frustum
        {
            get { return m_frustum; }
        }

        public Matrix GlobalMatrix
        {
            get { return m_globalMatrix; }
        }

        public Matrix UnscaledGlobalMatrix
        {
            get { return m_unscaledGlobalMatrix; }
        }

        public Matrix InverseRotationMatrix
        {
            get { return m_inverseRotationMatrix; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public CameraNode( string name )
            : base( name )
        {

        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void PreRenderUpdate( )
        {
            // First update its global matrix (if its a child of anything).
            GenerateGlobalMatrix(ref m_globalMatrix );

            RecalculateUnscaledGlobalMatrix();
            RecalculateGlobalRotationMatrix();


            // Does the camera's projection matrix or frustum need updating?
            if( ProjectionMatrixNeedsUpdate )
            {
                UpdateProjectionMatrix();
            }

            RecalculateViewAndProjectionMatrices();

            RecalculateFrustum();
        }


        public void RecalculateFrustum( )
        {
            m_frustum.Matrix = m_viewProjectionMatrix;
        }

        //public Vector2 GetScreenPosition( Vector3 worldPos )
        //{
        //    Vector2 normScreenPos = GetNormalisedScreenPosition( worldPos );

        //    return new Vector2( ( normScreenPos.X * (float)m_renderViewPort.HalfWidth ) + m_renderViewPort.HalfWidth,
        //                        m_renderViewPort.HalfHeight - ( normScreenPos.Y * (float)m_renderViewPort.HalfHeight ) );
        //}

        //public Vector3 GetWorldPosition( float screenX, float screenY, float depth )
        //{
        //    Ray ray = GetRayFromCamera( screenX, screenY );

        //    return m_globalMatrix.Translation + ray.Position + (ray.Direction * depth);
        //}


        // --------------------------------------------------------------------
        // -- Public Abstract Methods
        // --------------------------------------------------------------------
        public abstract void UpdateProjectionMatrix( );
        //public abstract Ray GetRayFromCamera( Point point );
        //public abstract Ray GetRayFromCamera( float x, float y );
        public abstract Vector2 GetNormalisedScreenPosition( Vector3 worldPos );
        public abstract Vector2 GetScreenPosition(Vector3 worldPos);
        public abstract bool IsVisible( Vector3 position, float radius );


        // --------------------------------------------------------------------
        // -- Private Methods
        // --------------------------------------------------------------------
        private void RecalculateUnscaledGlobalMatrix( )
        {
            m_unscaledGlobalMatrix = m_globalMatrix;

            float xScale = m_unscaledGlobalMatrix.Left.Length();
            float yScale = m_unscaledGlobalMatrix.Up.Length();
            float zScale = m_unscaledGlobalMatrix.Forward.Length();

            m_unscaledGlobalMatrix.Left /= xScale;
            m_unscaledGlobalMatrix.Up /= yScale;
            m_unscaledGlobalMatrix.Forward /= zScale;
            m_unscaledGlobalMatrix.Translation = m_globalMatrix.Translation;
        }


        private void RecalculateGlobalRotationMatrix()
        {
            m_rotationMatrix = m_unscaledGlobalMatrix;
            m_rotationMatrix.Translation = Vector3.Zero;

            // Calculate the inverse rotation matrix.
            m_inverseRotationMatrix = Matrix.Invert(m_rotationMatrix);
        }


        private void RecalculateViewAndProjectionMatrices()
        {
            m_viewMatrix = Matrix.Invert(m_unscaledGlobalMatrix);
            m_viewProjectionMatrix = m_viewMatrix * m_projectionMatrix;
            m_inverseViewProjectionMatrix = Matrix.Invert(m_viewProjectionMatrix);
        }
    }
}
