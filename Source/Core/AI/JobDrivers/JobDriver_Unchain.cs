using PrisonLabor.Constants;
using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobDrivers
{
    abstract class JobDriver_Unchain : JobDriver
	{
		private int ticks = 0;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed);
		}

		protected IEnumerable<Toil> MakeNewToils(HediffDef hediffDef)
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			//this.FailOnDowned(TargetIndex.A);
			this.FailOnAggroMentalState(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil wait = new Toil();
			wait.initAction = delegate
			{
				Pawn actor2 = wait.actor;
				Pawn pawn2 = (Pawn)job.GetTarget(TargetIndex.A).Thing;
				actor2.pather.StopDead();
				PawnUtility.ForceWait(pawn2, 15000, null, maintainPosture: true);
			};
			wait.tickAction = delegate
			{
				ticks++;
			};
			wait.AddFinishAction(delegate
			{
				Pawn pawn = (Pawn)job.GetTarget(TargetIndex.A).Thing;
				if (pawn != null)
				{
					Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef, false);
					if (hediff == null)
					{
						hediff = HediffMaker.MakeHediff(hediffDef, pawn, null);
						pawn.health.AddHediff(hediff, null, null);
					}
					else
					{
						pawn.health.hediffSet.hediffs.Remove(hediff);
					}
                    CuffsTracker cuffsTracker = pawn.Map.GetComponent<CuffsTracker>();
					if (cuffsTracker != null)
					{
						if (hediffDef == PL_DefOf.PrisonLabor_RemovedHandscuffs)
						{
							cuffsTracker.handscuffTracker.Remove(pawn);
						}
						else
						{
							cuffsTracker.legscuffTracker.Remove(pawn);
						}
					}
					if (pawn.CurJobDef == JobDefOf.Wait_MaintainPosture)
					{
						pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
					}
				}
			});
			wait.FailOnDespawnedOrNull(TargetIndex.A);
			wait.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			wait.AddEndCondition(() =>
			{
				if (ticks >= BGP.Unchain_Ticks)
				{
					return JobCondition.Succeeded;
				}
				return JobCondition.Ongoing;
			}
			);
			wait.defaultCompleteMode = ToilCompleteMode.Never;
			wait.WithProgressBar(TargetIndex.A, () => ticks / BGP.Unchain_Ticks);
			yield return wait;
		}
	}
}
