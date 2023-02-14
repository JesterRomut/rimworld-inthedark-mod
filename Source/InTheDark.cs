using System;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Runtime;

namespace InTheDark
{
    [StaticConstructorOnStartup]
    public class Startup
    {
        [StaticConstructorOnStartup]
        public static class Textures
        {
            public static readonly Texture2D BlackHoleEclipse = ContentFinder<Texture2D>.Get("UI/BlackHoleEclipse");
            public static readonly Texture2D VoidSpawnControlGroupGizmo = ContentFinder<Texture2D>.Get("UI/Abilities/VoidSpawnControlGroupGizmo");
        }
        
        static Startup()
        {
            HarmonyPatches.Init();

            Log.Message("<color=#84FFF2>In the Dark Mod loaded. Ready to get new endless & emptiness experience.</color>");

            ModCompatibilityPatch();
        }
        public static void ModCompatibilityPatch()
        {
            Type CompHauledToInventory = GenTypes.GetTypeInAnyAssembly("PickUpAndHaul.CompHauledToInventory", (string)null);
            if (CompHauledToInventory != null)
            {
                //object haulWorkGiver = Activator.CreateInstance(pickupAndHaul);
                ModCompatibility.PickUpAndHaul.enabled = true;
                ModCompatibility.PickUpAndHaul.CompHauledToInventory = CompHauledToInventory;
                ModCompatibility.PickUpAndHaul.RegisterHauledItem = CompHauledToInventory.GetMethod("RegisterHauledItem");
                Log.Message("In the Dark & Pick up and Haul compatibility applied.");
            }
        }
    }

    public class InTheDark_Mod : Mod
    {

        private static InTheDark_Mod _instance;
        public static InTheDark_Settings modSettings;

        public static InTheDark_Mod Main
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("Accessing InTheDarkMod before it was constructed.");
                }
                return _instance;
            }
        }

        public InTheDark_Mod(ModContentPack content) : base(content)
        {
            _instance = this;
            //Log.Message("<color=#84FFF2>In the Dark @build-2023210</color>");
            base.GetSettings<InTheDark_Settings>();
        }

        public override string SettingsCategory()
        {
            return "<color=#84FFF2>In The Dark</color>";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {

            InTheDark_Settings.DoSettingsWindowContents(inRect);
        }

    }

    public static class ModCompatibility
    {
        public static class PickUpAndHaul
        {
            public static bool enabled = false;
            public static Type CompHauledToInventory;
            public static MethodInfo RegisterHauledItem;
        }
    }

    public class VoidSpawnUtility
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