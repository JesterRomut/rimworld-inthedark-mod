using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace InTheDark
{
    public class CompProperties_VoidSpawn : CompProperties
    {
        public List<string> immnunity = null;
        public CompProperties_VoidSpawn()
        {
            this.compClass = typeof(CompVoidSpawn);
        }
    }
    public class CompVoidSpawn : ThingComp
    {
        private void AddVoidHediffAndMore()
        {
            Pawn pawn = this.parent as Pawn;
            //modify hediff
            if (pawn.health != null && pawn.health.hediffSet != null)
            {
                HediffDef hediffdef = HediffDef.Named("VoidSpawn");
                if (pawn.health.hediffSet.GetFirstHediffOfDef(hediffdef) == null)
                {
                    pawn.health.AddHediff(hediffdef);
                }

                foreach (string hediff in Props.immnunity)
                {
                    Hediff hediffToRemove = pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed(hediff, false));
                    if (hediffToRemove != null)
                    {
                        pawn.health.RemoveHediff(hediffToRemove);
                    }

                }
            }
            //add ability
            pawn.abilities?.GainAbility(VoidSpawnAbilityDefOf.VoidSpawnSkip);
        }
        public CompProperties_VoidSpawn Props
        {
            get
            {
                return (CompProperties_VoidSpawn)this.props;
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            VoidSpawnCollectionClass.AddVoidSpawnToList(this.parent);
            AddVoidHediffAndMore();
        }

        public override void PostDeSpawn(Map map)
        {
            VoidSpawnCollectionClass.RemoveVoidSpawnToList(this.parent);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            VoidSpawnCollectionClass.RemoveVoidSpawnToList(this.parent);
        }

        public override void CompTickRare()
        {

            base.CompTickRare();

            AddVoidHediffAndMore();
        }
    }
}