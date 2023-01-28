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
        public int gatheringIntervalTicks = 30000;
        public ThingDef productDef = null;
        public int productAmount = 1;
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
                HediffDef hediffdef = VoidSpawnHediffDefOf.VoidSpawnEthereal;
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
        private void GatherProduct()
        {
            Pawn pawn = this.parent as Pawn;
            Thing thing = ThingMaker.MakeThing(Props.productDef);
            thing.stackCount = Props.productAmount;
            if (pawn.IsCaravanMember())
            {
                Caravan caravan = pawn.GetCaravan();
                CaravanInventoryUtility.GiveThing(caravan, thing);
            }
            else
            {
                GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                //pawn.inventory.TryAddItemNotForSale(thing);
            }
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
        public override void CompTick()
        {
            base.CompTick();

            if (parent.IsHashIntervalTick(Props.gatheringIntervalTicks))
            {
                GatherProduct();
            }
        }
        public override void CompTickRare()
        {

            base.CompTickRare();

            AddVoidHediffAndMore();
        }
    }

}