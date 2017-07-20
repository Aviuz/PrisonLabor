using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;

namespace PrisonLabor
{
    class JobGiver_BedTime : ThinkNode_JobGiver
    {
        private RestCategory minCategory;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_BedTime jobGiver_BedTime = (JobGiver_BedTime)base.DeepCopy(resolve);
            jobGiver_BedTime.minCategory = this.minCategory;
            return jobGiver_BedTime;
        }

        public override float GetPriority(Pawn pawn)
        {
            if (pawn.timetable != null && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Sleep)
                return 10f;
            else
                return 0f; 
                    
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if(pawn.timetable == null || pawn.timetable.CurrentAssignment != TimeAssignmentDefOf.Sleep)
            {
                return null;
            }
            if (RestUtility.DisturbancePreventsLyingDown(pawn))
            {
                return null;
            }
            Lord lord = pawn.GetLord();
            Building_Bed building_Bed;
            if (lord != null && lord.CurLordToil != null && !lord.CurLordToil.AllowRestingInBed)
            {
                building_Bed = null;
            }
            else
            {
                building_Bed = RestUtility.FindBedFor(pawn);
            }
            if (building_Bed != null)
            {
                return new Job(JobDefOf.LayDown, building_Bed);
            }
            return new Job(JobDefOf.LayDown, this.FindGroundSleepSpotFor(pawn));
        }

        private IntVec3 FindGroundSleepSpotFor(Pawn pawn)
        {
            Map map = pawn.Map;
            for (int i = 0; i < 2; i++)
            {
                int radius = (i != 0) ? 12 : 4;
                IntVec3 result;
                if (CellFinder.TryRandomClosewalkCellNear(pawn.Position, map, radius, out result, (IntVec3 x) => !x.IsForbidden(pawn) && !x.GetTerrain(map).avoidWander))
                {
                    return result;
                }
            }
            return CellFinder.RandomClosewalkCellNearNotForbidden(pawn.Position, map, 4, pawn);
        }
    }
}
