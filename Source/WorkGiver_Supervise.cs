using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    internal class WorkGiver_Supervise : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var prisoner = t as Pawn;
            var need = prisoner.needs.TryGetNeed<Need_Motivation>();

            if (need == null || prisoner == null)
                return null;
            if (!ShouldTakeCareOfPrisoner(pawn, prisoner))
                return null;
            if (prisoner.Downed || !pawn.CanReserve(t, 1, -1, null, false) || !prisoner.Awake())
                return null;
            if (pawn.IsPrisoner)
                return null;
            if (!PrisonLaborUtility.LaborEnabled(prisoner) || !PrisonLaborUtility.WorkTime(prisoner) || !need.NeedToBeInspired)
                if(!need.CanEscape)
                    return null;

            return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerSupervise"), prisoner);
        }
    }
}