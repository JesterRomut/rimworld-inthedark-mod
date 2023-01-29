using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System;
using Verse.AI;

namespace InTheDark
{
    static partial class HarmonyPatches
    {
        [HarmonyPatch(typeof(Pawn_PathFollower))]
        [HarmonyPatch("CostToMoveIntoCell")]
        [HarmonyPatch(new Type[] { typeof(Pawn), typeof(IntVec3) })]
        [HarmonyPostfix]
        public static void DisableMoveCostForVoidSpawns(Pawn pawn, IntVec3 c, ref int __result)

        {

            if ((pawn.Map != null) && pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {

                int num;
                if (c.x == pawn.Position.x || c.z == pawn.Position.z)
                {
                    num = pawn.TicksPerMoveCardinal;
                }
                else
                {
                    num = pawn.TicksPerMoveDiagonal;
                }
                TerrainDef terrainDef = pawn.Map.terrainGrid.TerrainAt(c);
                if (terrainDef == null)
                {
                    num = 10000;
                }
                else if ((terrainDef.passability == Traversability.Impassable) && !terrainDef.IsWater)
                {
                    num = 10000;
                }
                List<Thing> list = pawn.Map.thingGrid.ThingsListAt(c);
                for (int i = 0; i < list.Count; i++)
                {
                    Thing thing = list[i];
                    if (thing.def.passability == Traversability.Impassable)
                    {
                        num = 10000;
                    }


                }

                __result = num;

            }

        }




        }
    }