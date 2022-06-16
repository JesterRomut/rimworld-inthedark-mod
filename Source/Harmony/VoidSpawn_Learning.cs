using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace InTheDark
{
    static partial class HarmonyPatches
    {
        [HarmonyPatch(typeof(SkillRecord))]
        [HarmonyPatch("Learn")]
        [HarmonyPatch(new Type[] { typeof(float), typeof(bool) })]
        [HarmonyPrefix]
        public static void DontLearnSkillWithoutPassionForVoidSpawns(SkillRecord __instance, ref float xp, bool direct/*, ref float __result*/)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (!VoidSpawnCollectionClass.void_spawns.Contains(pawn))
            {
                return;
            }
            //Log.Message($"void spawn learning detected, xp: {xp}");
            if (__instance.passion == Passion.None)
            {
                xp = 0;
                return;
            }
            if(xp < 0)
            {
                xp /= 2;
            }
        }
    }
    /*[HarmonyPatch(typeof(Pawn_NeedsTracker))]
    [HarmonyPatch("ShouldHaveNeed")]
    [HarmonyPatch(new Type[] { typeof(Pawn_NeedsTracker), typeof(NeedDef) })]
    public static class VoidSpawn_ShouldHaveNeed
    {

        public static void Postfix(Pawn_NeedsTracker __instance, NeedDef needdef, ref bool __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (VoidSpawnCollectionClass.void_spawns.Contains(pawn))
            {
                if (needdef == NeedDefOf.Food)
                {
                    __result = false;
                    return;
                }
            }
        }




    }

    [HarmonyPatch(typeof(ThinkNode_ConditionalNeedPercentageAbove))]
    [HarmonyPatch("Satisfied")]
    [HarmonyPatch(new Type[] { typeof(ThinkNode_ConditionalNeedPercentageAbove), typeof(Pawn) })]
    public static class VoidSpawn_Satisfied
    {
        public static bool Postfix(ThinkNode_ConditionalNeedPercentageAbove __instance, Pawn pawn,
            ref bool __result)
        {
            if (VoidSpawnCollectionClass.void_spawns.Contains(pawn) &&
                Traverse.Create(__instance).Field("need").GetValue<NeedDef>() == NeedDefOf.Food)
            {
                __result = true;
                return false;
            }

            return true;
        }

    }*/
}