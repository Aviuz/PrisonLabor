using System;
using System.Collections.Generic;
using PrisonLabor.Constants;
using PrisonLabor.Core.LaborArea;
using PrisonLabor.Core.LaborWorkSettings;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Other;
using RimWorld;
using Verse;

namespace PrisonLabor.Core
{
    public static class PrisonLaborUtility
    {
        public static bool LaborEnabled(this Pawn pawn)
        {
            if (pawn.IsPrisoner)
                if (pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workOption || pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndRecruitOption
                    || pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndConvertOption || pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndEnslaveOption)
                {
                    return true;
                }

            return false;
        }

        public static bool RecruitInLaborEnabled(Pawn pawn)
        {
            if (pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndRecruitOption && pawn.guest.ScheduledForInteraction)
            {
                return true;
            }

            return false;
        }

        public static bool ConvertInLaborEnabled(Pawn doer, Pawn prisoner)
        {
            if (prisoner.guest.interactionMode == PL_DefOf.PrisonLabor_workAndConvertOption && prisoner.guest.ScheduledForInteraction 
                && prisoner.Ideo != doer.Ideo && doer.Ideo == prisoner.guest.ideoForConversion)
            {
                return true;
            }
            return false;
        }

        public static bool EnslaveInLaborEnabled(Pawn doer, Pawn prisoner)
        {
            if (prisoner.guest.interactionMode == PL_DefOf.PrisonLabor_workAndEnslaveOption && prisoner.guest.ScheduledForInteraction
                && new HistoryEvent(HistoryEventDefOf.EnslavedPrisoner, doer.Named(HistoryEventArgsNames.Doer)).Notify_PawnAboutToDo_Job())
            {
                return true;
            }
            return false;
        }
        public static bool WorkTime(Pawn pawn)
        {
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

        public static bool CanWorkHere(IntVec3 pos, Pawn pawn, WorkTypeDef workType)
        {
            if (pawn.IsFreeNonSlaveColonist && pos != null && pawn.Map.areaManager.Get<Area_Labor>() != null && !WorkSettings.WorkDisabled(workType))
            {
                bool result = true;
                try
                {
                    result = !pawn.Map.areaManager.Get<Area_Labor>()[pos];
                }
                catch (IndexOutOfRangeException e)
                {
                    DebugLogger.debug($"{pawn.NameShortColored} cause IndexOutOfRangeException for {workType.label} calling pos {pos}");
                }
                return result;
            }
            return true;
        }

        public static Faction GetPawnFaction(Pawn pawn)
        {
            return pawn.IsPrisonerOfColony ? Faction.OfPlayer : pawn.Faction;
        }
    }
}