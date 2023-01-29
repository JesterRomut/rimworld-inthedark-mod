using RimWorld;
using Verse;
using System.Collections.Generic;
using Verse.Sound;
using UnityEngine;
using System;

namespace InTheDark
{
	public class CompProperties_AbilityVoidSpawnTeleport : CompProperties_EffectWithDest
	{
		public CompProperties_AbilityVoidSpawnTeleport()
		{
			this.compClass = typeof(CompAbilityEffect_VoidSpawnTeleport);
		}
	}
	public class CompAbilityEffect_VoidSpawnTeleport : CompAbilityEffect_WithDest
	{
		public new CompProperties_AbilityVoidSpawnTeleport Props
		{
			get
			{
				return (CompProperties_AbilityVoidSpawnTeleport)this.props;
			}
		}
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
            /*if (!dest.HasThing)
            {
				Log.Message("dest has no thing");
				return;
            }*/
			//Log.Message("Void Spawn teleporting detected");
			base.Apply(target, dest);
			//Log.Message($"{target.Cell} {dest.Cell}");
			/*if (destination.IsValid)
			{*/
			Pawn pawn = this.parent.pawn;
			IntVec3 destCell = target.Cell;
            //this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(pawn.Position, pawn.Map, 1f), pawn.Position, 60, null);
            //this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_ExitNoDelay.Spawn(destCell, pawn.Map, 1f), destCell, 60, null);
            VoidSpawnUtilty.SpawnSirenidaeFilth(pawn, pawn.Position, 1, new IntRange(3, 4));
            VoidSpawnUtilty.SpawnSirenidaeFilth(pawn, destCell, 1, new IntRange(3, 4));
            SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(pawn.Position, this.parent.pawn.Map, false));
            SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(destCell, this.parent.pawn.Map, false));
            pawn.Position = destCell;
			pawn.Notify_Teleported(true, true);
			//}
		}
		//public override IEnumerable<PreCastAction> GetPreCastActions()
		//{
		//	yield return new PreCastAction
		//	{
		//		action = delegate (LocalTargetInfo t, LocalTargetInfo d)
		//		{
		//			if (!this.parent.def.HasAreaOfEffect)
		//			{
		//				Pawn pawn = t.Pawn;
		//				if (pawn != null)
		//				{
		//					FleckCreationData dataAttachedOverlay = FleckMaker.GetDataAttachedOverlay(pawn, FleckDefOf.PsycastSkipFlashEntry, Vector3.zero, 1f, -1f);
		//					dataAttachedOverlay.link.detachAfterTicks = 5;
		//					pawn.Map.flecks.CreateFleck(dataAttachedOverlay);
		//				}
		//				else
		//				{
		//					FleckMaker.Static(t.CenterVector3, this.parent.pawn.Map, FleckDefOf.PsycastSkipFlashEntry, 1f);
		//				}
		//				FleckMaker.Static(d.Cell, this.parent.pawn.Map, FleckDefOf.PsycastSkipInnerExit, 1f);
		//			}
		//			if (this.Props.destination != AbilityEffectDestination.RandomInRange)
		//			{
		//				FleckMaker.Static(d.Cell, this.parent.pawn.Map, FleckDefOf.PsycastSkipOuterRingExit, 1f);
		//			}
		//			if (!this.parent.def.HasAreaOfEffect)
		//			{
		//				SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(t.Cell, this.parent.pawn.Map, false));
		//				SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(d.Cell, this.parent.pawn.Map, false));
		//			}
		//		},
		//		ticksAwayFromCast = 5
		//	};
		//	yield break;
		//}
		public override bool CanHitTarget(LocalTargetInfo target)
		{
			return base.CanPlaceSelectedTargetAt(target) && base.CanHitTarget(target);
		}
	}
	
}