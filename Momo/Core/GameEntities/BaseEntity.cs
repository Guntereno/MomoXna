using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Momo.Core.Nodes.Cameras;
using Momo.Core.Spatial;
using Momo.Debug;



namespace Momo.Core.GameEntities
{
    public abstract class BaseEntity : BinItem
    {
        // --------------------------------------------------------------------
        // -- Protected Members
        // --------------------------------------------------------------------
        protected Vector2 m_position = Vector2.Zero;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public virtual void SetPosition(Vector2 position)
        {
            m_position = position;
        }


        public virtual void IncrementPosition(Vector2 dPosition)
        {
            m_position += dPosition;
        }


        public override Vector2 GetPosition()
        {
            return m_position;
        }


        public Vector3 GetPosition3(float z)
        {
            return new Vector3(m_position, z);
        }


        public virtual void Update(ref FrameTime frameTime, uint updateToken)
        {

        }


        public virtual void PostUpdate()
        {

        }


        public virtual void Render(CameraNode camera, GraphicsDevice graphicsDevice)
        {

        }


        public virtual void DebugRender(DebugRenderer debugRenderer)
        {

        }
    }
}
