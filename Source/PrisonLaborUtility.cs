using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PrisonLabor
{
    internal class PrisonLaborUtility
    {
        private static PrisonerInteractionModeDef workDef;
        private static PrisonerInteractionModeDef workAndRecruitDef;

        public static bool LaborEnabled(Pawn pawn)
        {
            if (workDef == null || workAndRecruitDef == null)
            {
                workDef = DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption");
                workAndRecruitDef = DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workAndRecruitOption");
            }
            if (pawn.IsPrisoner && !PrisonLaborPrefs.DisableMod)
                if (pawn.guest.interactionMode == workDef || pawn.guest.interactionMode == workAndRecruitDef)
                    return true;

            return false;
        }

        public static bool RecruitInLaborEnabled(Pawn pawn)
        {
            if (workDef == null || workAndRecruitDef == null)
            {
                workDef = DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption");
                workAndRecruitDef = DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workAndRecruitOption");
            }
            if (pawn.guest.interactionMode == workAndRecruitDef && pawn.guest.ScheduledForInteraction && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
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
                    pawn.needs.food.CurCategory > HungerCategory.Hungry ||
                    pawn.needs.rest.CurCategory != RestCategory.Rested)
                    return false;
                else
                    return true;
            return false;
        }
    }
}