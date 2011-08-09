using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Primitive2D;
using Momo.Core.Spatial;
using Momo.Debug;

using Momo.Core.Collision2D;

using TestGame.Objects;



namespace TestGame.Entities
{
	public class AiEntity : DynamicGameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        static System.Random ms_random = new System.Random();

        float m_turnVelocity = 0.0f;

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public AiEntity()
        {
            FacingAngle = (float)ms_random.NextDouble() * ((float)Math.PI * 2.0f);


			SetContactRadiusInfo(new RadiusInfo(9.0f + ((float)ms_random.NextDouble() * 6.0f)));
        }

        public override void Update(ref FrameTime frameTime)
        {
            base.Update(ref frameTime);

            m_turnVelocity += ((float)ms_random.NextDouble() - 0.5f) * 50.0f * frameTime.Dt;
            m_turnVelocity = MathHelper.Clamp(m_turnVelocity, -1.0f, 1.0f);
            FacingAngle += m_turnVelocity * frameTime.Dt;

            Vector2 direction = new Vector2((float)Math.Cos(FacingAngle), (float)Math.Sin(FacingAngle));
            Vector2 newPosition = GetPosition() + direction;


            SetPosition(newPosition);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawCircle(GetPosition(), GetContactRadiusInfo().Radius, new Color(1.0f, 0.0f, 0.0f, 0.4f), new Color(0.2f, 0.0f, 0.0f, 0.75f), true, 2, 8);
        }



        public void AddToBin(Bin bin)
        {
            BinRegionUniform curBinRegion = new BinRegionUniform();
			bin.GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), ref curBinRegion);

            bin.UpdateBinItem(this, ref curBinRegion, 0);

            SetBinRegion(curBinRegion);
        }


        public void UpdateBinEntry(Bin bin)
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();

            GetBinRegion(ref prevBinRegion);
			bin.GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, 0);

            SetBinRegion(curBinRegion);
        }



        public override void OnCollisionEvent(ref IDynamicCollidable collidable)
        {

        }


        public void OnCollisionEvent(ref BulletEntity bullet)
        {
            SetForce(bullet.GetVelocity() * 10.0f);
        }


        public void OnExplosionEvent(ref Explosion explosion, Vector2 force)
        {
            SetForce(force);
        }
    }
}
