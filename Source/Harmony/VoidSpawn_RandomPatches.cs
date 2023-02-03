using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Unity.Jobs;
using Verse;
using Verse.AI;

namespace InTheDark
{
    static partial class HarmonyPatches
    {
        [HarmonyPatch(typeof(Pawn_AgeTracker))]
        [HarmonyPatch("LifeStageMinAge")]
        [HarmonyPatch(new Type[] { typeof(LifeStageDef) })]
        [HarmonyPrefix]
        public static bool DoNotCheckVoidSpawnLifeStageMinAge(Pawn_AgeTracker __instance, ref float __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (VoidSpawnCollectionClass.voidSpawns.Contains(pawn) || pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                __result = 0f;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PawnUtility))]
        [HarmonyPatch("IsInvisible")]
        [HarmonyPatch(new Type[] { typeof(Pawn) })]
        [HarmonyPostfix]
        public static void VoidSpawnInvisibility(ref Pawn pawn, ref bool __result)
        {
            if (VoidSpawnCollectionClass.voidSpawns.Contains(pawn))
            {
                __result = true;
            }
        }

        [HarmonyPatch(typeof(Pawn_GeneTracker))]
        [HarmonyPatch("AffectedByDarkness", MethodType.Getter)]
        [HarmonyPostfix]
        public static void VoidSpawnShouldNotAffectedByDarkness(Pawn_GeneTracker __instance, ref bool __result)
        {
            if (__instance.pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                __result = false;
            }
        }

        [HarmonyPatch(typeof(AbilityUtility))]
        [HarmonyPatch("ValidateMustBeHumanOrWildMan")]
        [HarmonyPatch(new Type[] { typeof(Pawn), typeof(bool), typeof(Ability) })]
        [HarmonyPostfix]
        public static void VoidSpawnAbilityFix(ref bool __result, Pawn targetPawn, bool showMessage, Ability ability)
        {
            if (targetPawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                __result = false;
            }
        }

        [HarmonyPatch(typeof(Building_Bed))]
        [HarmonyPatch("GetFloatMenuOptions")]
        [HarmonyPatch(new Type[] { typeof(Pawn) })]
        [HarmonyPostfix]
        public static IEnumerable<FloatMenuOption> VoidSpawnRestMenu(IEnumerable<FloatMenuOption> values, Building_Bed __instance, Pawn myPawn)
        {
            foreach (FloatMenuOption item in values)
            {
                yield return item;
            }
            if (myPawn.def == VoidSpawnThingDefOf.VoidSpawn_Race && __instance.Medical)
            {


                FloatMenuOption option = new FloatMenuOption("UseBedAsVoidSpawn".Translate(), delegate
                {
                    if (!__instance.ForPrisoners && myPawn.CanReserveAndReach(__instance, PathEndMode.ClosestTouch, Danger.Deadly, __instance.SleepingSlotsCount, -1, null, ignoreOtherReservations: true))
                    {
                        if (myPawn.CurJobDef == JobDefOf.LayDown && myPawn.CurJob.GetTarget(TargetIndex.A).Thing == __instance)
                        {
                            myPawn.CurJob.restUntilHealed = true;
                            myPawn.CurJob.forceSleep = true;
                        }
                        else
                        {
                            Job job = JobMaker.MakeJob(JobDefOf.LayDown, __instance);
                            job.restUntilHealed = true;
                            job.forceSleep = true;
                            myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        }
                        myPawn.mindState.ResetLastDisturbanceTick();
                    }
                });
                yield return option;

            }
        }

        [HarmonyPatch(typeof(ThinkNode_ConditionalMustKeepLyingDown))]
        [HarmonyPatch("Satisfied")]
        [HarmonyPatch(new Type[] { typeof(Pawn) })]
        [HarmonyPrefix]
        public static bool VoidSpawnKeepRest(ref bool __result, Pawn pawn)
        {
            if (pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race && (pawn.CurJob?.forceSleep ?? false))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
