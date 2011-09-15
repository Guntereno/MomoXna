using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Nodes.Cameras
{
    public class PerspectiveCameraNode : CameraNode
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private float m_fov = MathHelper.PiOver4;
        private float m_aspectRatio = 1.0f;


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public float FieldOfView
        {
            get { return m_fov; }
            set
            {
                m_fov = value;
                m_projectionMatrixNeedsUpdate = true;
            }
        }

        public float AspectRatio
        {
            get { return m_aspectRatio; }
            set
            {
                m_aspectRatio = value;
                m_projectionMatrixNeedsUpdate = true;
            }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public PerspectiveCameraNode( string name )
            : base( name )
        {

        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        //public void LookAt( Vector3 position ) { LookAt( position, new Vector3( 0.0f, 1.0f, 0.0f ) ); }
        //public void LookAt( Vector2 position, float zPlane ) { LookAt( new Vector3( position.X, position.Y, zPlane ), new Vector3( 0.0f, 1.0f, 0.0f ) ); }
        //public void LookAt( Vector3 position, Vector3 camerasLocalUp )
        //{
        //    // First update its global matrix (if its a child or anything).
        //    GenerateGlobalMatrix();

        //    // Set up the view matrix and projection matrix.
        //    Matrix globalMatrix = Matrix.CreateLookAt( this.GlobalTranslation, position, camerasLocalUp );

        //    // Update the local position.
        //    SetGlobalMatrix( globalMatrix );
        //}


        public override void UpdateProjectionMatrix( )
        {
            m_projectionMatrix = Matrix.CreatePerspectiveFieldOfView( m_fov, m_aspectRatio, m_nearClip, m_farClip );

            m_projectionMatrixNeedsUpdate = false;
        }


        //public override Ray GetRayFromCamera( Point point ) { return GetRayFromCamera( point.X, point.Y ); }
        //public override Ray GetRayFromCamera( float x, float y )
        //{
        //    Debug.Assert( m_renderViewPort != null, "CameraNode: No view port associated with camera. Can't calculate the ray!" );
        //    return m_renderViewPort.GetPickRay( x, y, m_projectionMatrix, m_viewMatrix, m_globalMatrix );
        //}


        public override Vector2 GetNormalisedScreenPosition( Vector3 worldPos )
        {
            worldPos.Z -= m_nearClip;
            Vector3 screenSpace = Vector3.Transform( worldPos, m_viewProjectionMatrix );

            return new Vector2( screenSpace.X / screenSpace.Z, screenSpace.Y / screenSpace.Z );
        }


        public override Vector2 GetScreenPosition(Vector3 worldPos)
        {
            return Vector2.Zero;
        }


        public override bool IsVisible( Vector3 position, float radius )
        {
            float leftDistance = m_frustum.Left.DotCoordinate( position ) - radius;
            float rightDistance = m_frustum.Right.DotCoordinate( position ) - radius;
            float topDistance = m_frustum.Top.DotCoordinate( position ) - radius;
            float bottomDistance = m_frustum.Bottom.DotCoordinate( position ) - radius;

            if( leftDistance > 0 || rightDistance > 0 || topDistance > 0 || bottomDistance > 0 )
                return false;

            return true;
        }

        // --------------------------------------------------------------------
        // -- Protected Methods
        // --------------------------------------------------------------------
    }
}
