using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PrisonLabor
{
    internal class JobGiver_BedTime : ThinkNode_JobGiver
    {
        private RestCategory minCategory;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var jobGiver_BedTime = (JobGiver_BedTime) base.DeepCopy(resolve);
            jobGiver_BedTime.minCategory = minCategory;
            return jobGiver_BedTime;
        }

        public override float GetPriority(Pawn pawn)
        {
            if (pawn.timetable != null && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Sleep)
                return 10f;
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.timetable == null || pawn.timetable.CurrentAssignment != TimeAssignmentDefOf.Sleep)
                return null;
            if (RestUtility.DisturbancePreventsLyingDown(pawn))
                return null;
            var need = pawn.needs.TryGetNeed<Need_Motivation>();
            if (need != null)
                need.PrisonerWorking = false;
            var lord = pawn.GetLord();
            Building_Bed building_Bed;
            if (lord != null && lord.CurLordToil != null && !lord.CurLordToil.AllowRestingInBed)
                building_Bed = null;
            else
                building_Bed = RestUtility.FindBedFor(pawn);
            if (building_Bed != null)
                return new Job(JobDefOf.LayDown, building_Bed);
            return new Job(JobDefOf.LayDown, FindGroundSleepSpotFor(pawn));
        }

        private IntVec3 FindGroundSleepSpotFor(Pawn pawn)
        {
            var map = pawn.Map;
            for (var i = 0; i < 2; i++)
            {
                var radius = i != 0 ? 12 : 4;
                IntVec3 result;
                if (CellFinder.TryRandomClosewalkCellNear(pawn.Position, map, radius, out result,
                    x => !x.IsForbidden(pawn) && !x.GetTerrain(map).avoidWander))
                    return result;
            }
            return CellFinder.RandomClosewalkCellNearNotForbidden(pawn.Position, map, 4, pawn);
        }
    }
}