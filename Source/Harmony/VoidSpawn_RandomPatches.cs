using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Unity.Jobs;
using UnityEngine;
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
            if (pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
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
            if (pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)// && !pawn.Drafted)
            {
                //if (pawn.Drafted)
                //{
                //    __result = false;
                //    return;
                //}
                if (pawn.health?.hediffSet?.GetFirstHediffOfDef(VoidSpawnHediffDefOf.VoidSpawnDoppelgangerWeakness) != null)
                {
                    __result = false;
                    return;
                }
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
            if (myPawn.def == VoidSpawnThingDefOf.VoidSpawn_Race && !__instance.Medical)
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
        [HarmonyPatch(typeof(PawnBreathMoteMaker))]
        [HarmonyPatch("TryMakeBreathMote")]
        [HarmonyPatch(new Type[] { })]
        [HarmonyPrefix]
        public static bool AVoidSpawnDoesNotFear(PawnBreathMoteMaker __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt))]
        [HarmonyPatch("RandomSelectionWeight")]
        [HarmonyPatch(new Type[] { typeof(Pawn), typeof(Pawn) })]
        [HarmonyPostfix]
        public static void VoidSpawnRomanceFix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator.def != VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                return;
            }

            //if (LovePartnerRelationUtility.HasAnyLovePartner(recipient))
            //{
            //    __result = 0f;
            //}
            __result = 0f;

        }

        [HarmonyPatch(typeof(Recipe_ExtractHemogen))]
        [HarmonyPatch("AvailableOnNow")]
        [HarmonyPatch(new Type[] { typeof(Thing), typeof(BodyPartRecord) })]
        [HarmonyPostfix]
        public static void VoidSpawnCannotExtractHemogen(ref bool __result, Thing thing)
        {
            if (thing is Pawn pawn && pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                __result = false;
            }
        }

        [HarmonyPatch(typeof(Need))]
        [HarmonyPatch("ShowOnNeedList", MethodType.Getter)]
        //[HarmonyPatch(new Type[] { typeof(Rect), typeof(int), typeof(float), typeof(bool), typeof(bool), typeof(Rect?), typeof(bool) })]
        [HarmonyPostfix]
        public static void HideVoidSpawnsFoodBar(Need __instance, ref bool __result)
        {
            if (!(__instance is Need_Food))
            {
                return;
            }
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                //return false;
                __result = false;
            }
            //return true;
        }

        [HarmonyPatch(typeof(MainMenuDrawer))]
        [HarmonyPatch("Init")]
        [HarmonyPatch(new Type[] { })]
        [HarmonyPostfix]
        public static void ApplyDarkenBackground()
        {
            if (InTheDark_Settings.useDarkenBackground)
            {
                ((UI_BackgroundMain)UIMenuBackgroundManager.background).overrideBGImage = Startup.Textures.BlackHoleEclipse;
            }
        }

        [HarmonyPatch(typeof(Pawn_GeneTracker))]
        [HarmonyPatch("GenesListForReading", MethodType.Getter)]
        [HarmonyPostfix]
        public static void HideVoidSpawnsGene(Pawn_GeneTracker __instance, ref List<Gene> __result)
        {
            if (__instance.pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                __result = new List<Gene>();
            }
        }

        [HarmonyPatch(typeof(Pawn_GeneTracker))]
        [HarmonyPatch("GetMelaninGene")]
        [HarmonyPatch(new Type[] { })]
        [HarmonyPostfix]
        public static void GetMelaninGenePatch(Pawn_GeneTracker __instance, ref GeneDef __result)
        {
            if (__instance.pawn.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                __result = VoidSpawnGeneDefOf.Skin_Melanin1;
            }
        }

        [HarmonyPatch(typeof(Pawn_HealthTracker))]
        [HarmonyPatch("AddHediff")]
        [HarmonyPatch(new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
        [HarmonyPostfix]
        public static void VoidSpawnImmunityPatch(Pawn_HealthTracker __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if(pawn.def != VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                return;
            }

            CompVoidSpawn comp = pawn.GetComp<CompVoidSpawn>();
            if (comp != null)
            {
                comp.RemoveUnnessaryHediff();
            }
        }
    }


}
