using System;

using Microsoft.Xna.Framework;



namespace Momo.Core.Spatial
{
    public class BinItem
    {
        internal Bin m_bin = null;
        internal BinRegionUniform m_region = BinRegionUniform.kInvalidBinRegionUniform;



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public Bin GetBin()
        {
            return m_bin;
        }

        public void SetBin(Bin bin)
        {
            m_bin = bin;
        }

        public void GetBinRegion(ref BinRegionUniform region)
        {
            region = m_region;
        }


        public void SetBinRegion(BinRegionUniform region)
        {
            m_region = region;
        }


        public void AddToBin(Bin bin, Vector2 corner1, Vector2 corner2, int binLayer)
        {
            bin.AddBinItem(this, corner1, corner2, binLayer);
            m_bin = bin;
        }


        public void AddToBin(Bin bin, Vector2 centre, float radius, int binLayer)
        {
            bin.AddBinItem(this, centre, radius, binLayer);
            m_bin = bin;
        }


        public void RemoveFromBin(int binLayer)
        {
            m_bin.RemoveBinItem(this, binLayer);
        }
    }
}
