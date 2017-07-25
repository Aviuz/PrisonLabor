using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    class WorkGiver_Supervise : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!base.ShouldTakeCareOfPrisoner(pawn, t) || ((Pawn)t).needs.TryGetNeed<Need_Motivation>() == null || !((Pawn)t).needs.TryGetNeed<Need_Motivation>().NeedToBeInspired)
            {
                return null;
            }
            if (!PrisonLaborUtility.LaborEnabled((Pawn)t) || !PrisonLaborUtility.WorkTime((Pawn)t))
            {
                return null;
            }
            Pawn pawn2 = (Pawn)t;
            if (!pawn2.Downed && pawn.CanReserve(t, 1, -1, null, false) && pawn2.Awake())
            {
                    return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerSupervise"), t);
            }
            return null;
        }
    }
}
