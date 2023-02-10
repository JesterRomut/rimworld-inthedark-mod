using RimWorld;
using Verse;
using System.Collections.Generic;
using Verse.Sound;
using UnityEngine;
using System;
using System.Linq;

namespace InTheDark
{
    public class Hediff_VoidSpawnCorruption : HediffWithComps
    {
        //public Pawn surgeon = null;
        public float TransformProgress
        {
            get
            {
                return Severity;
            }
            private set
            {
                Severity = value;
            }
        }
        public override void Tick()
        {
            ageTicks++;
            float num = PawnUtility.BodyResourceGrowthSpeed(pawn) * (float)Math.Max(pawn.needs.mood.MaxLevel - pawn.needs.mood.CurLevel, 0.2) / 100000f;
            TransformProgress += num;
            if (TransformProgress >= 1f)
            {
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    Messages.Message("MessageVoidSpawnTransform".Translate(pawn), pawn, MessageTypeDefOf.PositiveEvent);
                }
                DoTransform(pawn);
                pawn.health.RemoveHediff(this);
            }
        }
        //public static List<TraitDef> ExtractTraits(TraitSet traits)
        //{
        //    List<TraitDef> defs = new List<TraitDef>();
        //    foreach (Trait t in traits.allTraits) {
        //        if (t.sourceGene?.def != null && (t.sourceGene.def.forcedTraits == null || !t.sourceGene.def.forcedTraits.Any((GeneticTraitData x) => x.def == t.def && x.degree == t.Degree)))
        //        {
        //            continue;
        //        }
        //        defs.Append(t.def);
        //    }
        //    return defs;
        //}
        //public static List<TraitDef> ExtractSkillRecords(SkillRecord record, Pawn target)
        //{

        //}
        private static Pawn GenerateDoppelgangerFromPawn(Pawn pawn)
        {
            // Get faction & ideo
            Dictionary<Ideo, int> allIdeos = new Dictionary<Ideo, int>();
            Dictionary<Faction, int> allFactions = new Dictionary<Faction, int>();
            //int compare = 0;
            foreach (Pawn voidspawn in VoidSpawnGroupManager.Main.AllVoidSpawns)
            {
                Ideo currentIdeo = voidspawn.Ideo;
                if (!allIdeos.ContainsKey(currentIdeo))
                {
                    allIdeos.Add(currentIdeo, 0);
                }
                allIdeos[currentIdeo]++;

                Faction currentFaction = voidspawn.Faction;
                if (!allFactions.ContainsKey(currentFaction))
                {
                    allFactions.Add(currentFaction, 0);
                }
                allFactions[currentFaction]++;
            }
            Ideo chooseIdeo = allIdeos.MaxBy(kvp => kvp.Value).Key;
            Faction chooseFaction = allFactions.MaxBy(kvp => kvp.Value).Key;

            //foreach (KeyValuePair<Ideo, int> kv in allIdeos)
            //{
            //    if (kv.Value > compare)
            //    {
            //        compare = kv.Value;
            //        chooseIdeo = kv.Key;
            //    }
            //}
            //foreach (KeyValuePair<Faction, int> kv in allFactions)
            //{
            //    if (kv.Value > compare)
            //    {
            //        compare = kv.Value;
            //        chooseFaction = kv.Key;
            //    }
            //}

            // generate doppelganger
            PawnGenerationRequest request = new PawnGenerationRequest(
                VoidSpawnPawnKindDefOf.VoidSpawnDoppelganger,
                faction: chooseFaction,
                forceGenerateNewPawn: true,
                allowDead: false,
                allowDowned: false,
                canGeneratePawnRelations: false,
                allowFood: false,
                allowGay: false,
                fixedChronologicalAge: pawn.ageTracker.AgeChronologicalYears,
                fixedIdeo: chooseIdeo,
                allowAddictions: false
            );
            Pawn doppelganger = PawnGenerator.GeneratePawn(request);
            doppelganger.Name = new NameTriple(first: ((NameTriple)doppelganger.Name).First,nick: ((NameTriple)pawn.Name).First ?? pawn.Name.ToStringShort, last: ((NameTriple)doppelganger.Name).Last);
            //doppelganger.Name = NameTriple.FromString(NameGenerator.GenerateName(VoidSpawnRulePackDefOf.NamerVoidSpawnUniversal, (string x) => !NameTriple.FromString(x).UsedThisGame), true);
            // Extract story
            doppelganger.story.favoriteColor = pawn.story.favoriteColor;

            // Extract Traits
            doppelganger.story.traits.allTraits = new List<Trait>();
            foreach (Trait t in pawn.story.traits.allTraits)
            {
                if (t.sourceGene?.def == null)
                {
                    doppelganger.story.traits.allTraits.Add(new Trait(t.def, t.Degree));
                }
            }

            // Extract skills
            doppelganger.skills.skills = new List<SkillRecord>();
            foreach (SkillRecord sr in pawn.skills.skills)
            {
                SkillRecord srNew = new SkillRecord(doppelganger, sr.def);
                srNew.levelInt = sr.levelInt >= 0 ? sr.levelInt : 0;
                srNew.xpSinceLastLevel= sr.xpSinceLastLevel;
                srNew.passion = sr.passion;
                doppelganger.skills.skills.Add(srNew);
            }

            // Extract records
            DefMap<RecordDef, float> records = new DefMap<RecordDef, float>();
            foreach (KeyValuePair<RecordDef, float> recordPair in records)
            {
                // Log.Message(string.Concat(recordPair.Key, " value:", recordPair.Value));
                RecordDef key = recordPair.Key;
                if (key.type == RecordType.Time)
                {
                    continue;
                }
                doppelganger.records.AddTo(key, pawn.records.GetValue(key));
            }
            doppelganger.relations.AddDirectRelation(VoidSpawnPawnRelationDefOf.VoidSpawnDoppelganger, pawn);

            return doppelganger;
        }
        public static void DoTransform(Pawn pawn)
        {
            Pawn doppelganger = GenerateDoppelgangerFromPawn(pawn);

            HediffDef weakness = VoidSpawnHediffDefOf.VoidSpawnDoppelgangerWeakness;
            if (doppelganger.health.hediffSet.GetFirstHediffOfDef(weakness) == null)
            {
                doppelganger.health.AddHediff(weakness);
            }

            GenSpawn.Spawn(doppelganger, pawn.Position, pawn.Map);

            IntVec3 pos = pawn.Position;
            pawn.equipment.DropAllEquipment(pos, forbid: false);
            pawn.inventory.DropAllNearPawn(pos, forbid: false);

            pawn.DeSpawn();
            pawn.Destroy();
            if (doppelganger.Spawned)
            {
                VoidSpawnUtilty.SpawnSirenidaeFilth(doppelganger, doppelganger.PositionHeld, 2);
            }
            if (doppelganger.caller != null)
            {
                doppelganger.caller.DoCall();
            }
        }
    }
}