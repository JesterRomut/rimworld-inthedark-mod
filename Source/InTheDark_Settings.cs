using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace InTheDark
{
    public class InTheDark_Settings : ModSettings
    {
        public static bool useDarkenBackground = false;
        public static List<string> artworkRandomList = new List<string>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useDarkenBackground, "useDarkenBackground", defaultValue: false);
            Scribe_Collections.Look(ref artworkRandomList, "artworkRandomList", LookMode.Value);
        }
    }
}