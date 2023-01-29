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

        private void UpdateTarget()
        {
            Pawn pawn = this.parent.pawn;
            if (pawn.Spawned)
            {
                pawn.Map.attackTargetsCache.UpdateTarget(pawn);
            }
            PortraitsCache.SetDirty(pawn);
            GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
        }
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            UpdateTarget();
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            UpdateTarget();
        }

    }

}