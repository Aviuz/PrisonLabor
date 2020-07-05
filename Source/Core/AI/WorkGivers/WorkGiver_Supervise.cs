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

            var room = prisoner?.CurrentBed()?.GetRoom() ?? prisoner.GetRoom();
            if (room != null && prisoner.needs.food.CurLevelPercentage < 0.35 && !prisoner.Downed && prisoner.MentalState == null)
            {
                if (!PrisonFoodUtility.FoodAvailableInRoomFor(room, prisoner) && FoodUtility.TryFindBestFoodSourceFor(pawn, prisoner, false, out Thing foodSource, out ThingDef thingDef))
                {
                    float nutrition = FoodUtility.GetNutrition(foodSource, thingDef);
                    var job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerDeliverFoodSupervise"), foodSource, prisoner);


                    job.count = FoodUtility.WillIngestStackCountOf(prisoner, thingDef, nutrition);
                    job.targetC = RCellFinder.SpotToChewStandingNear(prisoner, foodSource);
                    return job;
                }
            }

            if (PrisonLaborUtility.RecruitInLaborEnabled(prisoner))
                return new Job(JobDefOf.PrisonerAttemptRecruit, t);

            if ((!PrisonLaborUtility.WorkTime(prisoner) || !mNeed.ShouldBeMotivated))
                return null;

            return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerSupervise"), prisoner);
        }
    }
}