using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapData;
using Momo.Core;

namespace TestGame.Entities
{
    public class PressurePlate : GameEntity
    {
        private PressurePlateData m_data;

        public PressurePlateData Data{ get{ return m_data; } }

        public PressurePlate(GameWorld world, PressurePlateData data)
            : base(world)
        {
            m_data = data;

            ContactRadiusInfo = new RadiusInfo(data.Radius);
            SetPosition(data.Position);

            // Initialise the bin
            //Bin bin = GetBin();
            //BinRegionUniform curBinRegion = new BinRegionUniform();
            //bin.GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + ContactDimensionPadding, ref curBinRegion);
            //SetBinRegion(curBinRegion);
        }
    }

    public class InteractivePressurePlate : PressurePlate
    {
        public InteractivePressurePlate(GameWorld world, PressurePlateData data)
            : base(world, data)
        {
        }
    }
}
