using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace InTheDark
{
    public class HediffCompProperties_VoidSpawnEthereal : HediffCompProperties
    {

        public HediffCompProperties_VoidSpawnEthereal()
        {
            compClass = typeof(HediffComp_VoidSpawnEthereal);
        }
    }
    public class HediffComp_VoidSpawnEthereal : HediffComp
    {
        public HediffCompProperties_VoidSpawnEthereal Props => (HediffCompProperties_VoidSpawnEthereal)props;
        //public override void CompPostTick(ref float severityAdjustment)
        //{
        //    base.CompPostTick(ref severityAdjustment);
        //    Pawn pawn = parent.pawn;
        //    if (pawn.IsHashIntervalTick(150) || pawn.needs == null || pawn.needs.mood == null || pawn.Faction == null)
        //    {
        //        return;
        //    }
        //    if (pawn.Spawned)
        //    {
        //        List<Pawn> pawns = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
        //        AffectPawns(pawn, pawns);
        //        return;
        //    }
        //    Caravan caravan = pawn.GetCaravan();
        //    if (caravan != null)
        //    {
        //        AffectPawns(pawn, caravan.pawns.InnerListForReading);
        //    }
        //}

        //private void AffectPawns(Pawn p, List<Pawn> pawns)
        //{
        //    for (int i = 0; i < pawns.Count; i++)
        //    {
        //        Pawn pawn = pawns[i];
        //        if (p == pawn || !(p.def == VoidSpawnThingDefOf.VoidSpawn_Race) || pawn == null || pawn.needs == null || pawn.needs.mood == null || pawn.needs.mood.thoughts == null)
        //        {
        //            continue;
        //        }
        //        bool flag = false;
        //        foreach (Thought_Memory memory in pawn.needs.mood.thoughts.memories.Memories)
        //        {
        //            if (memory is Thought_VoidSpawnThoughtSync checkThought && checkThought.harmonizer == parent)
        //            {
        //                checkThought.syncedMood = parent.pawn.needs.mood.CurLevel;
        //                flag = true;
        //                break;
        //            }
        //        }
        //        if (!flag)
        //        {
        //            Thought_VoidSpawnThoughtSync appliedThought = (Thought_VoidSpawnThoughtSync)ThoughtMaker.MakeThought(Props.thought);
        //            appliedThought.harmonizer = parent;
        //            appliedThought.otherPawn = parent.pawn;
        //            appliedThought.syncedMood = parent.pawn.needs.mood.CurLevel;
        //            pawn.needs.mood.thoughts.memories.TryGainMemory(appliedThought);
        //        }
        //    }
        //}

    }

}