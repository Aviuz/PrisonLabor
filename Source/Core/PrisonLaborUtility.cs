using System.Collections.Generic;
using PrisonLabor.Constants;
using PrisonLabor.Core.LaborArea;
using PrisonLabor.Core.LaborWorkSettings;
using PrisonLabor.Core.Meta;
using RimWorld;
using Verse;

namespace PrisonLabor.Core
{
    internal static class PrisonLaborUtility
    {
        public static bool LaborEnabled(this Pawn pawn)
        {
            if (pawn.IsPrisoner)
                if (pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workOption || pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndRecruitOption)
                    return true;

            return false;
        }

        public static bool RecruitInLaborEnabled(Pawn pawn)
        {
            if (pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndRecruitOption && pawn.guest.ScheduledForInteraction && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
                return true;

            return false;
        }

        public static bool WorkTime(Pawn pawn)
        {
            if (pawn.needs == null || pawn.needs.food == null || pawn.needs.rest == null)
                return false;
            if (pawn.timetable == null)
                return true;
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
                return true;
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything)
            {
                if (HealthAIUtility.ShouldSeekMedicalRest(pawn) ||
                    pawn.health.hediffSet.HasTemperatureInjury(TemperatureInjuryStage.Serious) ||
                    pawn.needs.food.CurCategory > HungerCategory.Hungry ||
                    pawn.needs.rest.CurCategory != RestCategory.Rested)
                    return false;
                else
                    return true;
            }
            return false;
        }

        public static bool IsDisabledByLabor(IntVec3 pos, Pawn pawn, WorkTypeDef workType)
        {
            if (pos != null && pawn.Map.areaManager.Get<Area_Labor>() != null &&
                !WorkSettings.WorkDisabled(workType))
                return pawn.Map.areaManager.Get<Area_Labor>()[pos];
            return false;
        }

        public static bool canWorkHere(IntVec3 pos, Pawn pawn, WorkTypeDef workType)
        {
            if (!pawn.IsPrisonerOfColony && pos != null && pawn.Map.areaManager.Get<Area_Labor>() != null &&
                !WorkSettings.WorkDisabled(workType))
                return !pawn.Map.areaManager.Get<Area_Labor>()[pos];
            return true;
        }
    }
}