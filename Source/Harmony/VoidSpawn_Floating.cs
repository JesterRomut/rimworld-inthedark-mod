using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System;
using Verse.AI;
using RimWorld.Planet;
using System.Text;

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
                foreach (Thing thing in list)
                {
                    if (thing.def.passability == Traversability.Impassable)
                    {
                        num = 10000;
                    }
                }
                //for (int i = 0; i < list.Count; i++)
                //{
                //    Thing thing = list[i];
                //    if (thing.def.passability == Traversability.Impassable)
                //    {
                //        num = 10000;
                //    }


                //}

                __result = num;

            }

        }
        [HarmonyPatch(typeof(Caravan_PathFollower))]
        [HarmonyPatch("CostToPayThisTick")]
        //[HarmonyPatch(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int?), typeof(bool), typeof(StringBuilder), typeof(string), typeof(bool) })]
        [HarmonyPostfix]
        public static void MoveCostForVoidSpawnsCaravan(Caravan_PathFollower __instance, ref float __result)
        {
            if (__instance == null)
            {
                //Log.Message("instance is null");
                return;
            }
            Caravan caravan = Traverse.Create(__instance).Field("caravan").GetValue<Caravan>();
            if (caravan == null)
            {
                //Log.Message("caravan is null");
                return;
            }
            List<Pawn> humanlikePawnsList = caravan.PawnsListForReading.FindAll((Pawn p) => p.RaceProps.Humanlike);
            if (humanlikePawnsList == null || !humanlikePawnsList.Any())
            {
                //Log.Message(string.Concat(string.Join(",", humanlikePawnsList), "not valid"));
                return;
            }
            List<Pawn> voidSpawnsList = humanlikePawnsList.FindAll((Pawn p) => p.def == VoidSpawnThingDefOf.VoidSpawn_Race);
            if (voidSpawnsList.Count == humanlikePawnsList.Count)
            {
                __result = Math.Max(__instance.nextTileCostTotal / 30000f, 25f);
                return;
                //Log.Message("all void spawn caravan");
            }
            if (voidSpawnsList.Any())
            {
                __result = Math.Max(__instance.nextTileCostTotal / 30000f, 10f);
            }
                //Log.Message("not all void spawn caravan");
        }


    }
    }