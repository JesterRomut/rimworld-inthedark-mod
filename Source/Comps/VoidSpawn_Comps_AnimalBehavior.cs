using System;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace InTheDark
{
    public class CompProperties_VoidSpawn: CompProperties
    {
        public CompProperties_VoidSpawn()
        {
            this.compClass = typeof(CompVoidSpawn);
        }
    }
    public class CompVoidSpawn: ThingComp
    {
        private void AddVoidHediffAndMore()
        {
            Pawn pawn = this.parent as Pawn;
            //add hediff
            HediffDef hediffdef = HediffDef.Named("VoidSpawn");
            if (pawn.health.hediffSet.GetFirstHediffOfDef(hediffdef) == null)
            {
                pawn.health.AddHediff(hediffdef);
                //pawn.health.hediffSet.GetFirstHediffOfDef(hediffdef).Severity += 1;
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