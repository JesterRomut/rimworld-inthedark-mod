using System;
using UnityEngine;
using Verse;

namespace InTheDark
{
    public class InTheDark_Settings : ModSettings
    {
        public static bool useDarkenBackground = false;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard li = new Listing_Standard();
            li.Begin(inRect);
            li.CheckboxLabeled("VoidSpawnSettingUseDarkenBackground".Translate(), ref useDarkenBackground, "VoidSpawnSettingUseDarkenBackgroundTooltip".Translate());
            li.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useDarkenBackground, "useDarkenBackground", defaultValue: false);
        }
    }
}