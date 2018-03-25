using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PrisonLabor
{
    internal class PrisonLaborUtility
    {
        private static PrisonerInteractionModeDef workDef;
        private static PrisonerInteractionModeDef workAndRecruitDef;

        private static List<WorkTypeDef> defaultWorkTypes;
        private static List<WorkTypeDef> allowedWorkTypes;

        private static List<WorkTypeDef> DefaultWorkTypes
        {
            get
            {
                if (defaultWorkTypes == null)
                {
                    defaultWorkTypes = new List<WorkTypeDef>();
                    defaultWorkTypes.Add(WorkTypeDefOf.Growing);
                    defaultWorkTypes.Add(WorkTypeDefOf.Mining);
                    defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("Hauling"));
                    defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("Cooking"));
                    defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("PlantCutting"));
                    defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("Crafting"));
                    defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("Cleaning"));

                    defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("HaulingUrgent", false));
                }
                return defaultWorkTypes;
            }
        }

        private static List<WorkTypeDef> AllowedWorkTypes
        {
            get
            {
                if (allowedWorkTypes == null)
                    return DefaultWorkTypes;
                return allowedWorkTypes;
            }
        }

        public static string AllowedWorkTypesData
        {
            get
            {
                if (allowedWorkTypes == null)
                    return "";
                var data = "";
                foreach (var workDef in allowedWorkTypes)
                    data += workDef.defName + ";";
                return data;
            }

            set
            {
                if (value.NullOrEmpty())
                {
                    allowedWorkTypes = null;
                }
                else
                {
                    allowedWorkTypes = new List<WorkTypeDef>();
                    var subs = value.Split(';');
                    foreach (var s in subs)
                        allowedWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed(s, false));
                }
            }
        }

        public static bool WorkDisabled(WorkTypeDef wt)
        {
            if (wt != null && !PrisonLaborPrefs.AllowAllWorkTypes)
                return !AllowedWorkTypes.Contains(wt);
            return false;
        }

        public static bool WorkDisabled(Pawn p, WorkTypeDef wt)
        {
            if (p.IsPrisoner)
                return WorkDisabled(wt);
            return false;
        }

        public static void SetAllowedWorkTypes(IEnumerable<WorkTypeDef> newList)
        {
            allowedWorkTypes = new List<WorkTypeDef>();
            foreach (var workDef in newList)
                allowedWorkTypes.Add(workDef);
        }

        public static void InitWorkSettings(Pawn pawn)
        {
            //Work Types
            if (!pawn.workSettings.EverWork)
                pawn.workSettings.EnableAndInitialize();
            foreach (var def in DefDatabase<WorkTypeDef>.AllDefs)
                if (WorkDisabled(def))
                    pawn.workSettings.Disable(def);

            //Timetables
            if (pawn.timetable == null)
                pawn.timetable = new Pawn_TimetableTracker(pawn);

            //Restrict areas
            pawn.playerSettings.AreaRestriction = null;
        }

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