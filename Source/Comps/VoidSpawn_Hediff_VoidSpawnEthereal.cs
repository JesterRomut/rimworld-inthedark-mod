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
        public ThoughtDef thought;

        public HediffCompProperties_VoidSpawnEthereal()
        {
            compClass = typeof(HediffComp_VoidSpawnEthereal);
        }
    }
    public class HediffComp_VoidSpawnEthereal : HediffComp
    {
        public HediffCompProperties_VoidSpawnEthereal Props => (HediffCompProperties_VoidSpawnEthereal)props;
    }
}