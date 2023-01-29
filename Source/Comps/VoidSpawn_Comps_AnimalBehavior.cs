using System;
using System.Collections.Generic;
using System.Linq;
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
        public ThoughtDef thought = null;
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
            if (!pawn.Spawned)
            {
                return;
            }
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
                if (pawn.Map == null)
                {
                    pawn.inventory.TryAddItemNotForSale(thing);
                }
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
            VoidSpawnCollectionClass.AddVoidSpawnToList(this.parent as Pawn);
            AddVoidHediffAndMore();
        }

        //public override void PostDeSpawn(Map map)
        //{
        //    VoidSpawnCollectionClass.RemoveVoidSpawnToList(this.parent as Pawn);
        //}

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            VoidSpawnCollectionClass.RemoveVoidSpawnToList(this.parent as Pawn);
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

            VoidSpawnCollectionClass.AddVoidSpawnToList(this.parent as Pawn);
        }
    }

    public class ThoughtWorker_VoidSpawnThoughtSync : ThoughtWorker
    {
        private float? average = null;
        //float? strippedMood = null;
        private float GetStrippedMood(Pawn p)
        {
            List<Thought> thoughts = new List<Thought>();
            //p.needs.mood.thoughts.situational.AppendMoodThoughts(thoughts);
            p.needs.mood.thoughts.GetAllMoodThoughts(thoughts);
            float totalMood = 0f;
            foreach(Thought t in thoughts)
            {
                if (t.def == VoidSpawnThoughtDefOf.VoidSpawnThoughtSync)
                {
                    continue;
                }
                totalMood += t.MoodOffset();
            }
            //if (p.IsColonist || p.IsPrisonerOfColony)
            //{
            //    totalMood += Find.Storyteller.difficulty.colonistMoodOffset;
            //}
            return totalMood / 100f;
        }

        private void CalculateAverage(Pawn p, IEnumerable<Pawn> voidSpawns)
        {
            if(voidSpawns.Count() == 0)
            {
                average = 0f;
            }
            float total = 0f;
            float count = 0f;
            foreach (Pawn siren in voidSpawns)
            {
                if (siren.def != VoidSpawnThingDefOf.VoidSpawn_Race)
                {
                    continue;
                }
                if (siren.Faction == p.Faction) { 
                    total += GetStrippedMood(siren);
                    count++;
                }
            }
            average = total / count;
        }

        //public override string PostProcessDescription(Pawn p, string description)
        //{
        //    return base.PostProcessDescription(p, description) + string.Concat(" In collectionclass: ", VoidSpawnCollectionClass.void_spawns.Contains(p), " all in collection: ", string.Join(",", VoidSpawnCollectionClass.void_spawns));
        //}
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            //Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(def.hediff);
            if (p.def == VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                return true;
                
            }
            return ThoughtState.Inactive;
        }
        public override float MoodMultiplier(Pawn p)
        {
            if (!p.IsHashIntervalTick(200) && average != null)
            {
                return average.Value - GetStrippedMood(p);
            }
            //Caravan caravan = p.GetCaravan();
            //if (caravan != null)
            //{
            //    List<Pawn> pawns = VoidSpawnCollectionClass.void_spawns.Concat(caravan.pawns.InnerListForReading).ToList();
            //    Log.Message(string.Concat(pawns.Count));
            //    CalculateAverage(p, pawns);
            //    return average.Value - GetStrippedMood(p);
            //}
            CalculateAverage(p, VoidSpawnCollectionClass.void_spawns);
            return average.Value - GetStrippedMood(p);
        }

    }

}