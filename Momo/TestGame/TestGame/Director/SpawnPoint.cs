using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Director
{
    public class SpawnPoint
    {
        public MapData.SpawnPointData Data { get; private set; }

        public SpawnPoint(MapData.SpawnPointData data)
        {
            Data = data;
        }
    }
}
