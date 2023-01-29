using System;
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
            Log.Message("In the Dark Mod loaded.");
        }
    }

    public class VoidSpawnCollectionClass
    {
        public static HashSet<Pawn> void_spawns = new HashSet<Pawn>();
        public static void AddVoidSpawnToList(Pawn thing)
        {
            if(!void_spawns.Contains(thing))
            {
                void_spawns.Add(thing);
            }
        }
        public static void RemoveVoidSpawnToList(Pawn thing)
        {
            if(void_spawns.Contains(thing))
            {
                void_spawns.Remove(thing);
            }
        }
    }

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