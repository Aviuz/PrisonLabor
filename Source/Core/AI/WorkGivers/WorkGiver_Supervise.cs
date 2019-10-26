using PrisonLabor.Core.Needs;
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
            var need = prisoner?.needs.TryGetNeed<Need_Motivation>();
            if (need == null || prisoner == null)
                return null;
            if (!ShouldTakeCareOfPrisoner(pawn, prisoner))
                return null;
            if (prisoner.Downed || !pawn.CanReserve(t, 1, -1, null, false) || !prisoner.Awake())
                return null;
            if (pawn.IsPrisoner)
                return null;
            var escapeTracker = EscapeTracker.Of(prisoner, true);
            if (!PrisonLaborUtility.LaborEnabled(prisoner) && !escapeTracker.CanEscape)
                return null;
            if (PrisonLaborUtility.RecruitInLaborEnabled(prisoner))
                return new Job(JobDefOf.PrisonerAttemptRecruit, t);
            if ((!PrisonLaborUtility.WorkTime(prisoner) || !need.ShouldToBeMotivated) && !escapeTracker.CanEscape)
                return null;

            return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerSupervise"), prisoner);
        }
    }
}