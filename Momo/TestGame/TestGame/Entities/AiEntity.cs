using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Primitive2D;
using Momo.Core.Spatial;
using Momo.Debug;



namespace TestGame.Entities
{
    public class AiEntity : DynamicGameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        static System.Random ms_random = new System.Random();

        float m_turnVelocity = 0.0f;
        float m_facingAngle = 0.0f;

        private CirclePrimitive2D m_collisionPrimitive = new CirclePrimitive2D(10.0f);



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public AiEntity()
        {
            m_facingAngle = (float)ms_random.NextDouble() * ((float)Math.PI * 2.0f);


            m_collisionPrimitive.Radius = 9.0f + ((float)ms_random.NextDouble() * 6.0f);
        }


        public CirclePrimitive2D GetCollisionPrimitive()
        {
            return m_collisionPrimitive;
        }


        public override void Update(ref FrameTime frameTime)
        {
            m_turnVelocity += ((float)ms_random.NextDouble() - 0.5f) * 50.0f * frameTime.Dt;
            m_turnVelocity = MathHelper.Clamp(m_turnVelocity, -1.0f, 1.0f);
            m_facingAngle += m_turnVelocity * frameTime.Dt;

            Vector2 direction = new Vector2((float)Math.Cos(m_facingAngle), (float)Math.Sin(m_facingAngle));
            Vector2 newPosition = GetPosition() + direction;


            SetPosition(newPosition);

            // Update the collision primitive
            m_collisionPrimitive.Centre = newPosition;
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawCircle(GetPosition(), m_collisionPrimitive.Radius, new Color(1.0f, 0.0f, 0.0f, 0.5f), new Color(0.0f, 0.0f, 0.0f, 0.75f), true, 2, 10);
        }



        public void AddToBin(Bin bin)
        {
            BinRegionUniform curBinRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(GetPosition(), m_collisionPrimitive.Radius, ref curBinRegion);

            bin.UpdateBinItemRegion(this, ref curBinRegion);
        }


        public void UpdateBinEntry(Bin bin)
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromCentre(GetPosition(), m_collisionPrimitive.Radius, ref curBinRegion);

            bin.UpdateBinItemRegion(this, ref prevBinRegion, ref curBinRegion);
        }
    }
}
