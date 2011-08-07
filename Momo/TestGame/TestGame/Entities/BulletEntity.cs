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
    public class BulletEntity : ProjectileGameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public BulletEntity()
        {

        }


        public override void Update(ref FrameTime frameTime)
        {
            base.Update(ref frameTime);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(GetPosition(), GetPosition() - GetVelocity() * 0.10f, new Color(0.0f, 1.0f, 1.0f, 0.6f), 5.0f);
        }


        public void UpdateBinEntry(Bin bin)
        {
            BinRegionUniform binRegion = new BinRegionUniform();

            bin.GetBinRegionFromUnsortedCorners(GetLastFramePosition(), GetPosition(), ref binRegion);

            SetBinRegion(binRegion);
        }
    }
}
