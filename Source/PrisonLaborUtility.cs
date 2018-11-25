using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PrisonLabor
{
    internal static class PrisonLaborUtility
    {
        public static bool LaborEnabled(this Pawn pawn)
        {
            if (pawn.IsPrisoner && !PrisonLaborPrefs.DisableMod)
                if (pawn.guest.interactionMode == PrisonLaborDefOf.PrisonLabor_workOption || pawn.guest.interactionMode == PrisonLaborDefOf.PrisonLabor_workAndRecruitOption)
                    return true;

            return false;
        }

        public static bool RecruitInLaborEnabled(Pawn pawn)
        {
            if (pawn.guest.interactionMode == PrisonLaborDefOf.PrisonLabor_workAndRecruitOption && pawn.guest.ScheduledForInteraction && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
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
                if (HealthAIUtility.ShouldSeekMedicalRest(pawn) || 
                    pawn.health.hediffSet.HasTemperatureInjury(TemperatureInjuryStage.Serious) ||
                    pawn.needs.food.CurCategory > HungerCategory.Hungry ||
                    pawn.needs.rest.CurCategory != RestCategory.Rested)
                    return false;
                else
                    return true;
            return false;
        }
    }
}