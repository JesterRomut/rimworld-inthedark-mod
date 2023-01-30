using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
            if (!(pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race))
            {
                return;
            }
            //Log.Message($"void spawn learning detected, xp: {xp}");
            if (__instance.passion == Passion.None)
            {
                xp = -666;
                return;
            }
            if(xp < 0)
            {
                xp /= 4;
            }
        }
        //[HarmonyPatch(typeof(Pawn_IdeoTracker))]
        //[HarmonyPatch("CertaintyChangeFactor", MethodType.Getter)]
        //[HarmonyPrefix]
        //public static bool DoNotCheckVoidSpawnCertaintyLifeStage(Pawn_IdeoTracker __instance, ref float __result)
        //{
        //    Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        //    if (VoidSpawnCollectionClass.void_spawns.Contains(pawn))
        //    {
        //        __result = 1f;
        //        return false;
        //    }

        //    return true;
        //    //__instance.pawnAgeCertaintyCurve
        //}
        [HarmonyPatch(typeof(Pawn_AgeTracker))]
        [HarmonyPatch("LifeStageMinAge")]
        [HarmonyPatch(new Type[] { typeof(LifeStageDef) })]
        [HarmonyPrefix]
        public static bool DoNotCheckVoidSpawnLifeStageMinAge(Pawn_AgeTracker __instance, ref float __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (VoidSpawnCollectionClass.void_spawns.Contains(pawn) || pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                __result = 0f;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PawnUtility))]
        [HarmonyPatch("IsInvisible")]
        [HarmonyPatch(new Type[] { typeof(Pawn) })]
        [HarmonyPrefix]
        public static bool VoidSpawnInvisibility(ref Pawn pawn, ref bool __result)
        {
            if (VoidSpawnCollectionClass.void_spawns.Contains(pawn))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Pawn_GeneTracker))]
        [HarmonyPatch("AffectedByDarkness", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool VoidSpawnShouldNotAffectedByDarkness(Pawn_GeneTracker __instance, ref bool __result)
        {
            if (__instance.pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                __result = false;
                return false;
            }
            return true;
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