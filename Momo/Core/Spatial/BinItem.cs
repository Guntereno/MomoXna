using System;

using Microsoft.Xna.Framework;



namespace Momo.Core.Spatial
{
    public abstract class BinItem
    {
        internal BinRegionUniform mRegion = BinRegionUniform.kInvalidBinRegionUniform;



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public abstract Vector2 GetPosition();



        public void GetBinRegion(ref BinRegionUniform region)
        {
            region = mRegion;
        }


        public BinRegionUniform GetBinRegion()
        {
            return mRegion;
        }


        public void SetBinRegion(BinRegionUniform region)
        {
            mRegion = region;
        }


        public void AddToBin(Bin bin, Vector2 corner1, Vector2 corner2, int binLayer)
        {
            bin.AddBinItem(this, corner1, corner2, binLayer);
        }


        public void AddToBin(Bin bin, Vector2 centre, float radius, int binLayer)
        {
            bin.AddBinItem(this, centre, radius, binLayer);
        }


        public void RemoveFromBin(Bin bin, int binLayer)
        {
            bin.RemoveBinItem(this, binLayer);
        }
    }
}
