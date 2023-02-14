using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace InTheDark
{
    public class ScenPart_ConfigPage_ConfigureStartingPawns_VoidSpawnWithInjured : ScenPart_ConfigPage_ConfigureStartingPawnsBase
    {
        public int sirenCount = 1;
        private string sirenCountBuffer;
        public int injuredCount = 1;
        private string injuredCountBuffer;

        private PawnKindDef injuredPawnKindDef;
        public PawnKindDef InjuredPawnKindDef
        {
            get
            {
                return injuredPawnKindDef ?? PawnKindDefOf.SpaceRefugee;
            }
            set
            {
                injuredPawnKindDef = value;
            }
        }

        private IEnumerable<PawnKindDef> AvailableKinds => DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef x) => x.RaceProps.Humanlike && x.race != VoidSpawnThingDefOf.VoidSpawn_Race && x.defaultFactionType != null && x.defaultFactionType.isPlayer);

        private string pawnCountChoiceBuffer;

        [MustTranslate]
        public string customSummary;

        protected override int TotalPawnCount => sirenCount + injuredCount;

        public override string Summary(Scenario scen)
        {
            return customSummary;
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            base.DoEditInterface(listing);
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 3f);
            scenPartRect.height = ScenPart.RowHeight;
            Text.Anchor = TextAnchor.UpperRight;
            Rect rect = new Rect(scenPartRect.x - 200f, scenPartRect.y + ScenPart.RowHeight, 200f, ScenPart.RowHeight);
            rect.xMax -= 4f;
            //Widgets.Label(rect, "ScenPart_StartWithPawns_OutOf".Translate());
            //Text.Anchor = TextAnchor.UpperLeft;
            Widgets.TextFieldNumeric(scenPartRect, ref sirenCount, ref sirenCountBuffer, 1f, 10f);
            scenPartRect.y += ScenPart.RowHeight;
            Widgets.TextFieldNumeric(scenPartRect, ref injuredCount, ref injuredCountBuffer, 1f, 10f);
            scenPartRect.y += ScenPart.RowHeight;
            Widgets.TextFieldNumeric(scenPartRect, ref pawnChoiceCount, ref pawnCountChoiceBuffer, TotalPawnCount, 10f);
            if (Widgets.ButtonText(rect, InjuredPawnKindDef.LabelCap))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (PawnKindDef availableKind in AvailableKinds)
                {
                    PawnKindDef localKind = availableKind;
                    list.Add(new FloatMenuOption(localKind.LabelCap, delegate
                    {
                        InjuredPawnKindDef = localKind;
                    }));
                }
                if (list.Any())
                {
                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }
        }

        protected override void GenerateStartingPawns()
        {
            int num = 0;
            do
            {
                StartingPawnUtility.ClearAllStartingPawns();

                int idx = 0;
                for (int i = 0; i < sirenCount; i++)
                {
                    PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(idx);
                    generationRequest.KindDef = VoidSpawnPawnKindDefOf.VoidSpawnColonist;
                    StartingPawnUtility.SetGenerationRequest(idx, generationRequest);
                    StartingPawnUtility.AddNewPawn(idx);
                    idx++;
                }
                for (int i = 0; i < injuredCount; i++)
                {
                    PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(idx);
                    generationRequest.KindDef = InjuredPawnKindDef;
                    StartingPawnUtility.SetGenerationRequest(idx, generationRequest);
                    StartingPawnUtility.AddNewPawn(idx);
                    idx++;
                }

                num++;
            }
            while (num <= 20 && !StartingPawnUtility.WorkTypeRequirementsSatisfied());
        }

        public override void PostGameStart()
        {
            base.PostGameStart();

            foreach (Pawn p in Find.GameInitData.startingAndOptionalPawns)
            {
                if (p.def == VoidSpawnThingDefOf.VoidSpawn_Race)
                {
                    continue;
                }
                HealthUtility.DamageUntilDowned(p);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref sirenCount, "sirenCount", 0);
            Scribe_Values.Look(ref injuredCount, "injuredCount", 0);
        }
    }
}