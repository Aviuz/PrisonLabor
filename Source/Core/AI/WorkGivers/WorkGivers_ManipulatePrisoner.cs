using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.WorkGivers
{
    class WorkGivers_ManipulatePrisoner : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var prisoner = t as Pawn;

            if (prisoner == null)
                return null;
            if (!ShouldTakeCareOfPrisoner(pawn, prisoner))
                return null;
            if (prisoner.Downed || !pawn.CanReserve(t, 1, -1, null, false) || !prisoner.Awake())
                return null;
            if (pawn.IsPrisoner || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
                return null;


            if (PrisonLaborUtility.RecruitInLaborEnabled(prisoner))
            {
                return new Job(JobDefOf.PrisonerAttemptRecruit, t);
            }
            if (PrisonLaborUtility.ConvertInLaborEnabled(pawn, prisoner))
            {
                return new Job(JobDefOf.PrisonerConvert, t);
            }
            if (PrisonLaborUtility.EnslaveInLaborEnabled(pawn, prisoner))
            {
                return new Job(JobDefOf.PrisonerEnslave, t);
            }

            return null;
        }
    }
}

