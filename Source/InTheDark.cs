using System;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Runtime;
using System.Linq;

namespace InTheDark
{
    [StaticConstructorOnStartup]
    public class Startup
    {
        [StaticConstructorOnStartup]
        public static class Textures
        {
            public static readonly Texture2D BlackHoleEclipse = ContentFinder<Texture2D>.Get("UI/Artworks/BlackHoleEclipse");
            public static readonly Texture2D InTheDark = ContentFinder<Texture2D>.Get("UI/Artworks/InTheDark");
            public static readonly Texture2D VoidSpawnControlGroupGizmo = ContentFinder<Texture2D>.Get("UI/Abilities/VoidSpawnControlGroupGizmo");
        }

        static Startup()
        {
            HarmonyPatches.Init();

            Log.Message("<color=#84FFF2>In the Dark Mod loaded.</color>");

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

        private static Vector2 leftScrollPosition = default;
        private static Vector2 rightScrollPosition = default;
        private static ArtworkDef leftSelectedDef;
        private static ArtworkDef rightSelectedDef;

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

            Listing_Standard li = new Listing_Standard();
            li.Begin(inRect);
            li.CheckboxLabeled("VoidSpawnSettingUseDarkenBackground".Translate(), ref InTheDark_Settings.useDarkenBackground, "VoidSpawnSettingUseDarkenBackgroundTooltip".Translate());
            li.End();

            if (InTheDark_Settings.useDarkenBackground)
            {

                Text.Font = GameFont.Small;
                Rect topRect = inRect.TopPart(pct: 0.05f);
                Rect labelRect = inRect.BottomPart(pct: 0.6f).TopHalf();
                Rect bottomRect = inRect.BottomPart(pct: 0.5f);

                Widgets.Label(rect: labelRect.LeftHalf().RightPart(pct: 0.9f), label: "VoidSpawnArtworkListAvaliable".Translate());
                Rect leftRect = bottomRect.LeftHalf().RightPart(pct: 0.9f).LeftPart(pct: 0.9f);
                GUI.BeginGroup(position: leftRect, style: new GUIStyle(other: GUI.skin.box));
                HashSet<ArtworkDef> found = DefDatabase<ArtworkDef>.AllDefs.ToHashSet();
                float num = 3f;
                Widgets.BeginScrollView(outRect: leftRect.AtZero(), scrollPosition: ref leftScrollPosition,
                                        viewRect: new Rect(x: 0f, y: 0f, width: leftRect.width / 10 * 9, height: found.Count * 32f));
                if (found.Any())
                {
                    foreach (ArtworkDef def in found)
                    {
                        Rect rowRect = new Rect(x: 5, y: num, width: leftRect.width - 6, height: 30);
                        Widgets.DrawHighlightIfMouseover(rect: rowRect);
                        if (def == leftSelectedDef)
                        {
                            Widgets.DrawHighlightSelected(rect: rowRect);
                        }
                        Widgets.Label(rect: rowRect, label: def.LabelCap.RawText ?? def.defName);
                        if (Widgets.ButtonInvisible(butRect: rowRect))
                        {
                            leftSelectedDef = def;
                        }

                        num += 32f;
                    }
                }

                Widgets.EndScrollView();
                GUI.EndGroup();

                Widgets.Label(rect: labelRect.RightHalf().RightPart(pct: 0.9f), label: "VoidSpawnArtworkListInRandom".Translate());
                Rect rightRect = bottomRect.RightHalf().RightPart(pct: 0.9f).LeftPart(pct: 0.9f);
                GUI.BeginGroup(position: rightRect, style: GUI.skin.box);
                num = 6f;
                Widgets.BeginScrollView(outRect: rightRect.AtZero(), scrollPosition: ref rightScrollPosition,
                                        viewRect: new Rect(x: 0f, y: 0f, width: rightRect.width / 5 * 4, height: InTheDark_Settings.artworkRandomList?.Count ?? 0 * 32f));
                if (InTheDark_Settings.artworkRandomList?.Any() ?? false)
                {
                    foreach (string defName in InTheDark_Settings.artworkRandomList)
                    {
                        ArtworkDef def = DefDatabase<ArtworkDef>.GetNamedSilentFail(defName);
                        if (def == null)
                        {
                            continue;
                        }
                        Rect rowRect = new Rect(x: 5, y: num, width: leftRect.width - 6, height: 30);
                        Widgets.DrawHighlightIfMouseover(rect: rowRect);
                        if (def == rightSelectedDef)
                        {
                            Widgets.DrawHighlightSelected(rect: rowRect);
                        }
                        Widgets.Label(rect: rowRect, label: def.LabelCap.RawText ?? def.defName);
                        if (Widgets.ButtonInvisible(butRect: rowRect))
                        {
                            rightSelectedDef = def;
                        }

                        num += 32f;
                    }
                }

                Widgets.EndScrollView();
                GUI.EndGroup();

                if (Widgets.ButtonImage(butRect: bottomRect.BottomPart(pct: 0.6f).TopPart(pct: 0.1f).RightPart(pct: 0.525f).LeftPart(pct: 0.1f), tex: TexUI.ArrowTexRight) &&
                leftSelectedDef != null)
                {
                    if (InTheDark_Settings.artworkRandomList == null)
                    {
                        InTheDark_Settings.artworkRandomList = new List<string>();
                    }
                    if (!InTheDark_Settings.artworkRandomList.Contains(leftSelectedDef.defName))
                    {
                        InTheDark_Settings.artworkRandomList.Add(item: leftSelectedDef.defName);
                        InTheDark_Settings.artworkRandomList = InTheDark_Settings.artworkRandomList.OrderBy(keySelector: td => td).ToList();
                        rightSelectedDef = leftSelectedDef;
                        leftSelectedDef = null;
                    }
                }

                if (Widgets.ButtonImage(butRect: bottomRect.BottomPart(pct: 0.4f).TopPart(pct: 0.15f).RightPart(pct: 0.525f).LeftPart(pct: 0.1f), tex: TexUI.ArrowTexLeft) &&
                    rightSelectedDef != null)
                {
                    InTheDark_Settings.artworkRandomList.Remove(item: rightSelectedDef.defName);
                    leftSelectedDef = rightSelectedDef;
                    rightSelectedDef = null;
                }
            }
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

        public static bool IsInvisible(Pawn pawn)
        {
            return !pawn.Drafted && pawn.health?.hediffSet?.GetFirstHediffOfDef(VoidSpawnHediffDefOf.VoidSpawnDoppelgangerWeakness) == null;
        }
    }

}