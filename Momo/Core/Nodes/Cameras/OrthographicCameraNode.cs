using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Nodes.Cameras
{
    public class OrthographicCameraNode : CameraNode
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private float m_viewWidth = 800.0f;
        private float m_viewHeight = 600.0f;
        private float m_halfViewWidth = 400.0f;
        private float m_halfViewHeight = 300.0f;


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public float ViewWidth
        {
            get { return m_viewWidth; }
            set
            {
                m_viewWidth = value;
                m_halfViewWidth = value * 0.5f;
                UpdateProjectionMatrix();
            }
        }

        public float ViewHeight
        {
            get { return m_viewHeight; }
            set
            {
                m_viewHeight = value;
                m_halfViewHeight = value * 0.5f;
                UpdateProjectionMatrix();
            }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public OrthographicCameraNode( string name )
            : base( name )
        {

        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        //public void LookAt( Vector3 position ) { LookAt( position.X, position.Y ); }
        //public void LookAt( Vector2 position ) { LookAt( position.X, position.Y ); }
        //public void LookAt( float x, float y )
        //{
        //    // First update its global matrix (if its a child or anything).
        //    GenerateGlobalMatrix();

        //    // Set up the view matrix and projection matrix.
        //    Matrix globalMatrix = Matrix.CreateTranslation( new Vector3( x, y, GlobalTranslation.Z ) );

        //    // Update the local position.
        //    SetGlobalMatrix( globalMatrix );
        //}


        public override void UpdateProjectionMatrix( )
        {
            m_projectionMatrix = Matrix.CreateOrthographic( m_viewWidth, m_viewHeight, m_nearClip, m_farClip );

            m_projectionMatrixNeedsUpdate = false;
        }


        //public override Ray GetRayFromCamera( Point point ) { return GetRayFromCamera( point.X, point.Y ); }
        //public override Ray GetRayFromCamera( float x, float y )
        //{
        //    Debug.Assert( m_renderViewPort != null, "CameraNode: No view port associated with camera. Can't calculate the ray!" );
        //    return m_renderViewPort.GetPickRay( x, y, m_projectionMatrix, m_viewMatrix, m_globalMatrix );
        //}


        public override Vector2 GetNormalisedScreenPosition(Vector3 worldPos)
        {
            Vector3 screenSpacePosition = Vector3.Transform(worldPos, ViewMatrix);

            return new Vector2(screenSpacePosition.X / m_viewWidth, -screenSpacePosition.Y / m_viewHeight);
        }


        public override Vector2 GetScreenPosition(Vector3 worldPos)
        {
            Vector3 screenSpacePosition = Vector3.Transform(worldPos, ViewMatrix);

            return new Vector2(screenSpacePosition.X, -screenSpacePosition.Y);
        }


        public override bool IsVisible( Vector3 position, float radius )
        {
            float left = position.X - radius;
            float right = position.X + radius;
            float top = position.Y - radius;
            float bottom = position.Y + radius;

            if( right < -m_halfViewWidth || left > m_halfViewWidth || bottom < -m_halfViewHeight || top > m_halfViewHeight )
                return false;

            return true;
        }


        // --------------------------------------------------------------------
        // -- Private Methods
        // --------------------------------------------------------------------


        // --------------------------------------------------------------------
        // -- Protected Methods
        // --------------------------------------------------------------------
    }
}
