using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Spatial;
using Momo.Debug;

using MapData;

using Game.Entities;
using Game.Entities.AI;



namespace Game
{
    namespace Director
    {

        public class Director
        {
            public Zone CurrentZone { get; private set; }

            public List<SpawnPoint> SpawnPoints { get; private set; }


            private AiEntity[] mAmbientEntitiesList;

            private BinRegionSelection mAmbientSpawnBinRegions;
            private BinQueryResults mAmbientSpawnQuery;



            private float mAmbientSpawnTimer = 0.0f;
            private float mAmbientSpawnFrequency = 1.0f;
            private int mAmbientSpawnAmount = 10;
            private ulong mDoNotSpawnTickCnt = 3000;



            public void Init()
            {
                mAmbientEntitiesList = new AiEntity[10];
                mAmbientSpawnBinRegions = new BinRegionSelection(1000);
                mAmbientSpawnQuery = new BinQueryResults(1000);

                // Make it spawn at the start.
                mAmbientSpawnTimer = mAmbientSpawnFrequency;
            }


            public void Update(ref FrameTime frameTime)
            {
                if (CurrentZone != null)
                {
                    if (mAmbientSpawnTimer > mAmbientSpawnFrequency)
                    {
                        TryAmbientSpawn(mAmbientSpawnAmount);
                        mAmbientSpawnTimer -= mAmbientSpawnFrequency;
                    }

                    mAmbientSpawnTimer += frameTime.Dt;
                }
            }


            private bool TryAmbientSpawn(int count)
            {
                int spawnCnt = 0;
                bool success = CurrentZone.RequestEntities(typeof(Zombie), count, ref mAmbientEntitiesList);


                if (success)
                {
                    Vector2 centre = CurrentZone.PlayerManager.AveragePlayerPosition;
                    PopulateAmbientSpawnPoints(centre, new Vector2(1250.0f, 820.0f));


                    mAmbientSpawnQuery.StartQuery();
                    CurrentZone.Bin.Query(ref mAmbientSpawnBinRegions, BinLayers.kAmbientSpawnPoints, mAmbientSpawnQuery);
                    mAmbientSpawnQuery.EndQuery();

                    if (mAmbientSpawnQuery.BinItemCount > 0)
                    {
                        int tryCnt = count * 5;
                        while (spawnCnt < count && tryCnt > 0)
                        {
                            SpawnPointData spawnPoint = (SpawnPointData)mAmbientSpawnQuery.BinItemQueryResults[CurrentZone.Random.Next(mAmbientSpawnQuery.BinItemCount)];

                            if (!CollisionHelpers.CheckForContacts(spawnPoint.Position, 0.02f, CurrentZone.Bin, BinLayers.kEntityList))
                            {
                                mAmbientEntitiesList[spawnCnt].SetPosition(spawnPoint.Position);
                                mAmbientEntitiesList[spawnCnt].AddToBin();
                                ++spawnCnt;
                            }

                            --tryCnt;
                        }
                    }


                    for (int i = spawnCnt; i < count; ++i)
                    {
                        mAmbientEntitiesList[spawnCnt].DestroyItem();
                        ++spawnCnt;
                    }
                }


                return success;
            }


            private void PopulateAmbientSpawnPoints(Vector2 centre, Vector2 screenDimensions)
            {
                Vector2 halfScreenDimensions = screenDimensions * 0.5f;
                Vector2 minCorner = centre - halfScreenDimensions;
                Vector2 maxCorner = centre + halfScreenDimensions;
                
                
                BinLocation minBinLocation = new BinLocation();
                BinLocation maxBinLocation = new BinLocation();

                CurrentZone.Bin.GetBinLocation(minCorner, ref minBinLocation);
                CurrentZone.Bin.GetBinLocation(maxCorner, ref maxBinLocation);

                mAmbientSpawnBinRegions.Clear();



                for (int depth = 0; depth < 2; ++depth)
                {
                    int index = CurrentZone.Bin.GetBinIndex(ref minBinLocation);


                    for (int x = minBinLocation.X; x < maxBinLocation.X; ++x)
                    {
                        PopulateAmbientSpawnPoints(index, centre);
                        ++index;
                    }

                    for (int y = minBinLocation.Y; y < maxBinLocation.Y; ++y)
                    {
                        PopulateAmbientSpawnPoints(index, centre);
                        index += CurrentZone.Bin.BinCountX;
                    }

                    for (int x = minBinLocation.X; x < maxBinLocation.X; ++x)
                    {
                        PopulateAmbientSpawnPoints(index, centre);
                        --index;
                    }
                    for (int y = minBinLocation.Y; y < maxBinLocation.Y; ++y)
                    {
                        PopulateAmbientSpawnPoints(index, centre);
                        index -= CurrentZone.Bin.BinCountX;
                    }

                    minBinLocation.X = minBinLocation.X - 1;
                    minBinLocation.Y = minBinLocation.Y - 1;

                    maxBinLocation.X = maxBinLocation.X + 1;
                    maxBinLocation.Y = maxBinLocation.Y + 1;
                }
            }


            private void PopulateAmbientSpawnPoints(int binIndex, Vector2 centre)
            {
                //Vector2 binCentre = CurrentZone.Bin.GetCentrePositionOfBin(binIndex);

                //Vector2 dCentre = binCentre - centre;

                //for (int i = 0; i < CurrentZone.PlayerManager.Players.ActiveItemListCount; ++i)
                //{
                    //float dotFacing = Vector2.Dot(CurrentZone.PlayerManager.Players[i].FacingDirection, dCentre);
                    //if (dotFacing > 0.0f)
                    //{
                    //    mAmbientSpawnBinRegions.AddBinIndex(binIndex);
                    //}
                //}

                ulong timeDifference = CurrentZone.World.CurrentTimeStamp - CurrentZone.BinPlayerHeatMap.Query(binIndex);

                if (timeDifference > mDoNotSpawnTickCnt)
                {
                    mAmbientSpawnBinRegions.AddBinIndex(binIndex);
                }
            }


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
                    Color ambientSpawnRegions = new Color(1.0f, 0.0f, 0.0f, 0.8f);

                    Color edge = new Color(0.0f, 1.0f, 1.0f, 0.3f);
                    Color fill = edge;

                    for (int i = 0; i < SpawnPoints.Count; ++i)
                    {
                        SpawnPoint spawnPoint = SpawnPoints[i];

                        Vector2 pos = spawnPoint.Data.Position;
                        double orient = (double)spawnPoint.Data.Orientation;
                        const float kDirLen = 24.0f;
                        Vector2 dir = new Vector2(-(float)Math.Sin(orient), -(float)Math.Cos(orient));

                        debugRenderer.DrawCircle(pos, 16.0f, edge, fill, false, 0.01f, 6);
                        debugRenderer.DrawLine(pos, pos + (dir * kDirLen), edge);
                    }


                    //for (int i = 0; i < mAmbientSpawnBinRegions.BinCount; ++i)
                    //{
                    //    Vector2 binCentre = CurrentZone.Bin.GetCentrePositionOfBin(mAmbientSpawnBinRegions.BinIndices[i]);
                    //    debugRenderer.DrawFilledCircle(binCentre, 32.0f, ambientSpawnRegions);
                    //}
                }
            }
        }
    }
}