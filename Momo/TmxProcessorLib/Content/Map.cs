using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TmxProcessorLib.Content
{
    public class Map
    {
        internal int LayerCount { get; set; }

        internal List<Tileset> Tilesets { get; set; }
        internal Dictionary<uint, Tile> Tiles { get; set; }

        internal List<List<Vector2>> CollisionBoundaries { get; set; }

        internal List<Vector2> PlayerSpawns { get; set; }

        internal Vector2 MinPlayableArea { get; set; }
        internal Vector2 MaxPlayableArea { get; set; }

        internal List<SpawnPoint> SpawnPoints { get; set; }

        internal List<List<Patch>> Patches { get; set; }

        internal List<ModelInst> SceneObjects { get; set; }

        //internal List<TimerEvent> TimerEvents { get; private set; }
        //internal List<SpawnEvent> SpawnEvents { get; private set; }
        //internal List<TriggerCounterEvent> TriggerCounterEvents { get; private set; }

        public void Write(ContentWriter output)
        {
            output.Write(LayerCount);

            output.Write(Tilesets.Count);
            foreach (Tileset tileset in Tilesets)
            {
                tileset.Write(output);
            }

            // Output the collision boundaries
            if (CollisionBoundaries != null)
            {
                output.Write(CollisionBoundaries.Count);
                foreach (List<Vector2> boundary in CollisionBoundaries)
                {
                    output.Write(boundary.Count);

                    foreach (Vector2 pos in boundary)
                    {
                        output.WriteObject<Vector2>(pos);
                    }
                }
            }
            else
            {
                output.Write(0);
            }

            // Output the player spawns
            output.Write(PlayerSpawns.Count);
            foreach (Vector2 pos in PlayerSpawns)
            {
                output.Write(pos);
            }

            // Output each spawn point
            output.Write(SpawnPoints.Count);
            foreach (SpawnPoint spawnPoint in SpawnPoints)
            {
                spawnPoint.Write(output);
            }

            // Output the play area definition
            output.Write(MinPlayableArea);
            output.Write(MaxPlayableArea);

            // Output the patch objects
            foreach (List<Patch> patchLayer in Patches)
            {
                output.Write(patchLayer.Count);
                foreach (Patch patch in patchLayer)
                {
                    patch.Write(output);
                }
            }

            // Output the scene objects
            output.Write(SceneObjects.Count);
            foreach (ModelInst modelInst in SceneObjects)
            {              
                modelInst.Write(output);
            }

            /*
                        // Output the pressure plates
                        output.Write(PressurePlates.Count);
                        for (int plateNum = 0; plateNum < PressurePlates.Count; ++plateNum)
                        {
                            Object plate = PressurePlates[plateNum];
                            int type;
                            if (plate.Type == "")
                            {
                                type = (int)(MapData.PressurePlateData.Type.Normal);
                            }
                            else
                            {
                                type = (int)(Enum.Parse(typeof(MapData.PressurePlateData.Type), plate.Type, true));
                            }

                            // Output defaults
                            output.Write(type);
                            output.Write(plate.Name);
                            output.Write(plate.Position + Offset);

                            // Use the largest dimension as the diameter of the circle
                            float radius = Math.Max(plate.Dimensions.X, plate.Dimensions.Y) * 0.5f;
                            output.Write(radius);

                            if(!plate.Properties.Properties.ContainsKey("trigger"))
                            {
                                throw new InvalidContentException("Pressure plate missing 'trigger' property!");
                            }
                            output.Write(plate.Properties.Properties["trigger"]);

                            switch (plate.Type)
                            {
                                case "Interactive":
                                    if(!plate.Properties.Properties.ContainsKey("interactTime"))
                                    {
                                        throw new InvalidContentException("Interactive pressure plate missing 'interactTime' property!");
                                    }
                                    output.Write(float.Parse(plate.Properties.Properties["interactTime"]));
                                    break;

                                case "Normal":
                                case null:
                                    // Do nothing
                                    break;

                                default:
                                    throw new InvalidContentException("Invalid pressure plate type!");
                            }
                        }

                        // Output the events
                        // Timers
                        output.Write(TimerEvents.Count);
                        foreach( TimerEvent timerEvent in TimerEvents)
                        {
                            timerEvent.Write(output);
                        }

                        // Spawns
                        output.Write(SpawnEvents.Count);
                        foreach (SpawnEvent spawnEvent in SpawnEvents)
                        {
                            spawnEvent.Write(output);
                        }

                        // Trigger Counters
                        output.Write(TriggerCounterEvents.Count);
                        foreach (TriggerCounterEvent triggerCounterEvent in TriggerCounterEvents)
                        {
                            triggerCounterEvent.Write(output);
                        }
             */
        }
    }
}
