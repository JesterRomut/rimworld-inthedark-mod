using System;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;

namespace InTheDark
{
    [StaticConstructorOnStartup]
    public class Startup
    {
        static Startup()
        {
            HarmonyPatches.Init();

            Log.Message("<color=#84FFF2>In the Dark Mod loaded. Ready to get new endless & emptiness experience.</color>");

            PickUpAndHaulPatch();
        }
        public static void PickUpAndHaulPatch()
        {
            Type pickupAndHaul = GenTypes.GetTypeInAnyAssembly("PickUpAndHaul.WorkGiver_HaulToInventory", (string)null);
            if (pickupAndHaul != null)
            {
                WorkGiver_HaulGeneral haulWorkGiver = (WorkGiver_HaulGeneral)Activator.CreateInstance(pickupAndHaul);
                VoidSpawnUtilty.HaulJobGlobalDelegate = (Pawn pawn, Thing thing, bool forced) => haulWorkGiver.ShouldSkip(pawn, forced) ? null : haulWorkGiver.JobOnThing(pawn, thing, forced);
                Log.Message("In the Dark & Pick up and Haul compatibility applied.");
            }
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
        public delegate Job HaulJobGlobal(Pawn pawn, Thing t, bool forced);
        public static HaulJobGlobal HaulJobGlobalDelegate = (Pawn pawn, Thing t, bool forced) => HaulAIUtility.HaulToStorageJob(pawn, t);
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