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
        public static HashSet<Thing> void_spawns = new HashSet<Thing>();
        public static void AddVoidSpawnToList(Thing thing)
        {
            if(!void_spawns.Contains(thing))
            {
                void_spawns.Add(thing);
            }
        }
        public static void RemoveVoidSpawnToList(Thing thing)
        {
            if(void_spawns.Contains(thing))
            {
                void_spawns.Remove(thing);
            }
        }
    }

}