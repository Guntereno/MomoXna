using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Debug;
using Microsoft.Xna.Framework;

namespace Game
{
    namespace Director
    {

        public class Director
        {
            public Zone CurrentZone { get; private set; }

            public List<SpawnPoint> SpawnPoints { get; private set; }

            public void LoadZone(Zone zone)
            {
                if (CurrentZone == zone)
                    return;

                CurrentZone = zone;

                if (CurrentZone != null)
                {
                    SpawnPoints = new List<SpawnPoint>();
                    for(int i=0; i<CurrentZone.Map.SpawnPoints.Length; ++i)
                    {
                        MapData.SpawnPointData spawnPointData = CurrentZone.Map.SpawnPoints[i];
                        SpawnPoints.Add(new SpawnPoint(spawnPointData));
                    }
                }
            }

            public void DebugRender(DebugRenderer debugRenderer)
            {
                if (CurrentZone != null)
                {
                    Color edge = new Color(0.0f, 1.0f, 1.0f, 0.3f); ;
                    Color fill = edge;

                    for (int i = 0; i < SpawnPoints.Count; ++i)
                    {
                        SpawnPoint spawnPoint = SpawnPoints[i];

                        Vector2 pos = spawnPoint.Data.Position;
                        double orient = (double)spawnPoint.Data.Orientation;
                        const float kDirLen = 24.0f;
                        Vector2 dir = new Vector2(-(float)Math.Sin(orient), -(float)Math.Cos(orient));

                        debugRenderer.DrawCircle(pos, 16.0f, edge, fill, true, 0.01f, 6);
                        debugRenderer.DrawLine(pos, pos + (dir * kDirLen), edge);
                    }
                }
            }
        }
    }
}