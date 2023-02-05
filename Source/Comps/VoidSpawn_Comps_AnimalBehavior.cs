using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;
using static HarmonyLib.Code;

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

            //modify skin color
            pawn.story.skinColorOverride = new Color(1f, 1f, 1f, 0.85f);
        }

        private void GatherProduct()
        {
            Pawn pawn = this.parent as Pawn;
            if (pawn.needs?.mood?.CurLevel / pawn.needs?.mood?.MaxLevel < 0.5f)
            {
                // only gather above 50% mood
                return;
            }
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
                else
                {
                    GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                }

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
            Pawn pawn = this.parent as Pawn;
            if (pawn.Faction == Faction.OfPlayer)
            {
                if (VoidSpawnGroupManager.Main.GetControlGroup(pawn) == null)
                {
                    VoidSpawnGroupManager.Main.ControlGroups[0].Assign(pawn);
                }
            }
            //VoidSpawnCollectionClass.AddVoidSpawnToList(pawn);
            AddVoidHediffAndMore();
        }

        //public override void PostDeSpawn(Map map)
        //{
        //    VoidSpawnCollectionClass.RemoveVoidSpawnToList(this.parent as Pawn);
        //}

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            Pawn pawn = this.parent as Pawn;
            VoidSpawnGroupManager.Main.GetControlGroup(pawn)?.TryUnassign(pawn);
            //VoidSpawnCollectionClass.RemoveVoidSpawnToList(pawn);
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

            //VoidSpawnCollectionClass.AddVoidSpawnToList(this.parent as Pawn);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Pawn pawn = this.parent as Pawn;
            if (pawn != null && (pawn.Faction ?? null) == Faction.OfPlayer)
            {
                yield return new VoidSpawnControlGroupGizmo(pawn);
            }

            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }

            yield break;
        }
    }

    

}