using System.Collections.Generic;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.WorkGivers
{
    internal class WorkGiver_Supervise : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var prisoner = t as Pawn;
            var mNeed = prisoner?.needs.TryGetNeed<Need_Motivation>();

            if (pawn.IsPrisoner)
                return null;

            if (mNeed == null || prisoner == null || prisoner.Dead)
                return null;

            if (!ShouldTakeCareOfPrisoner(pawn, prisoner))
                return null;

            if ((!prisoner.InBed() && prisoner.Downed) || !pawn.CanReserve(t, 1, -1, null, false) || !prisoner.Awake())
                return null;

            if (prisoner.needs.food.CurLevelPercentage < 0.25 && !PrisonFoodUtility.FoodAvailableInRoomTo(prisoner) && prisoner.guest.CanBeBroughtFood && pawn.jobs.curJob == null)
                return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerDeliverFoodSupervise"), prisoner);

            if (PrisonLaborUtility.RecruitInLaborEnabled(prisoner))
                return new Job(JobDefOf.PrisonerAttemptRecruit, t);

            if ((!PrisonLaborUtility.WorkTime(prisoner) || !mNeed.ShouldBeMotivated))
                return null;

            return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerSupervise"), prisoner);
        }
    }
}