using System;
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
                if (pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workOption || pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndRecruitOption || pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndConvertOption)
                    return true;

            return false;
        }

        public static bool RecruitInLaborEnabled(Pawn pawn)
        {
            if (pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndRecruitOption && pawn.guest.ScheduledForInteraction && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
                return true;

            return false;
        }

        public static bool ConvertInLaborEnabled(Pawn pawn)
        {
            if (pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndConvertOption && pawn.guest.ScheduledForInteraction && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
                return true;

            return false;
        }

        public static bool WorkTime(Pawn pawn)
        {
/*            if (pawn.needs == null || pawn.needs.food == null || pawn.needs.rest == null)
                return false;*/
            if (pawn.timetable == null)
                return true;
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
                return true;
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything)
            {
                if (HealthAIUtility.ShouldSeekMedicalRest(pawn) ||
                    pawn.health.hediffSet.HasTemperatureInjury(TemperatureInjuryStage.Serious) ||
                    CheckFoodNeed(pawn) ||
                    CheckRestNeed(pawn))
                    return false;
                else
                    return true;
            }
            return false;
        }

        private static bool CheckFoodNeed(Pawn pawn)
        {
            return pawn.needs != null && pawn.needs.food != null && pawn.needs.food.CurCategory > HungerCategory.Hungry;
        }

        private static bool CheckRestNeed(Pawn pawn)
        {
            return pawn.needs != null && pawn.needs.rest != null && pawn.needs.rest.CurCategory != RestCategory.Rested;
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
            {
                bool result = true;
                try
                {
                    result = !pawn.Map.areaManager.Get<Area_Labor>()[pos];
                }
                catch (IndexOutOfRangeException e)
                {
                    Log.Message($"IndexOutOfRangeException for {workType.label} calling pos {pos}");
                }
                return result;
            }
            return true;
        }
    }
}