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
            if (pawn.def != VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                return;
            }
            //Log.Message($"void spawn learning detected, xp: {xp}");
            if (__instance.passion == Passion.None)
            {
                xp = -666;
                return;
            }
            if (xp < 0)
            {
                xp /= 4;
            }
        }

    }
}