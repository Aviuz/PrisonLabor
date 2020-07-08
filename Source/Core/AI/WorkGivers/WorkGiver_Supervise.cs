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

            if ((!prisoner.InBed() && prisoner.Downed) || !prisoner.Awake())
                return null;

            if (PrisonLaborUtility.RecruitInLaborEnabled(prisoner))
                return new Job(JobDefOf.PrisonerAttemptRecruit, t);

            if (prisoner.needs.food.CurLevelPercentage < 0.15 && !prisoner.Downed)
            {
                var curRoom = prisoner?.GetRoom();
                var room = prisoner?.CurrentBed()?.GetRoom() ?? curRoom;

                if (room != null && pawn.CanReserve(t, 1, -1, null, false))
                {
                    if (curRoom == null)
                        goto CheckFood;

                    if (curRoom.isPrisonCell)
                        goto SkipFood;


                    CheckFood:

                    if (!PrisonFoodUtility.FoodAvailableInRoomFor(room, prisoner))
                    {

                        if (FoodUtility.TryFindBestFoodSourceFor(pawn, prisoner, false, out Thing foodSource, out ThingDef thingDef))
                        {
                            float nutrition = FoodUtility.GetNutrition(foodSource, thingDef);
                            Job nJob = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerDeliverFoodSupervise"), foodSource, prisoner);

                            nJob.count = FoodUtility.WillIngestStackCountOf(prisoner, thingDef, nutrition);
                            if (!room.isPrisonCell)
                                nJob.targetC = RCellFinder.SpotToChewStandingNear(prisoner, foodSource);
                            else
                                nJob.targetC = room.Cells.RandomElement();
                            return nJob;
                        }
                    }
                }
            }

        SkipFood:

            if (!PrisonLaborUtility.WorkTime(prisoner) || !mNeed.ShouldBeMotivated || !pawn.CanReserve(t, 1, -1, null, false))
                return null;

            return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_PrisonerSupervise"), prisoner);
        }
    }
}