﻿using System;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace InTheDark
{
    [StaticConstructorOnStartup]
    public class Startup
    {
        static Startup()
        {
            HarmonyPatches.Init();
            Log.Message("In the Dark Mod loaded. Ready to get new endless & emptiness experience.");
        }
    }

    //public class VoidSpawnCollectionClass
    //{
    //    public static HashSet<Pawn> voidSpawns = new HashSet<Pawn>();
    //    public static void AddVoidSpawnToList(Pawn thing)
    //    {
    //        if(!voidSpawns.Contains(thing))
    //        {
    //            voidSpawns.Add(thing);
    //        }
    //    }
    //    public static void RemoveVoidSpawnToList(Pawn thing)
    //    {
    //        if(voidSpawns.Contains(thing))
    //        {
    //            voidSpawns.Remove(thing);
    //        }
    //    }
    //}

    public class VoidSpawnUtilty
    {
        public static void SpawnSirenidaeFilth(Pawn pawn, IntVec3 center, int radius, IntRange? amount = null)
        {
            int randomInRange = (amount ?? new IntRange(6, 10)).RandomInRange;
            for (int i = 0; i < randomInRange; i++)
            {
                FilthMaker.TryMakeFilth(CellFinder.RandomClosewalkCellNear(center, pawn.Map, radius), pawn.Map, VoidSpawnThingDefOf.Filth_Sirenidae, pawn.LabelIndefinite());
            }
        }
    }

}