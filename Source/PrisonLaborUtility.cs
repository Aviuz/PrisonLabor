using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    class PrisonLaborUtility
    {

        private static PrisonerInteractionModeDef pimDef;

        private static List<WorkTypeDef> defaultWorkTypes;
        private static List<WorkTypeDef> allowedWorkTypes;

        private static List<WorkTypeDef> DefaultWorkTypes
        {
            get
            {
                if(defaultWorkTypes == null)
                {
                    defaultWorkTypes = new List<WorkTypeDef>();
                    defaultWorkTypes.Add(WorkTypeDefOf.Growing);
                    defaultWorkTypes.Add(WorkTypeDefOf.Mining);
                    defaultWorkTypes.Add(WorkTypeDefOf.Hauling);
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
                {
                    return DefaultWorkTypes;
                }
                else
                {
                    return allowedWorkTypes;
                }
            }
        }

        public static string AllowedWorkTypesData
        {
            get
            {
                if (allowedWorkTypes == null)
                {
                    return "";
                }
                else
                {
                    string data = "";
                    foreach(WorkTypeDef workDef in allowedWorkTypes)
                    {
                        data += workDef.defName + ";";
                    }
                    return data;
                }
            }

            set
            {
                if(value.NullOrEmpty())
                {
                    allowedWorkTypes = null;
                }
                else
                {
                    allowedWorkTypes = new List<WorkTypeDef>();
                    string[] subs = value.Split(';');
                    foreach (string s in subs)
                        allowedWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed(s, false));
                }
            }
        }

        public static bool WorkDisabled(WorkTypeDef wt)
        {
            if (wt != null && !PrisonLaborPrefs.AllowAllWorkTypes)
                return !AllowedWorkTypes.Contains(wt);
            else
                return false;
        }

        public static bool WorkDisabled(Pawn p, WorkTypeDef wt)
        {
            if (p.IsPrisoner)
                return WorkDisabled(wt);
            else
                return false;
        }

        public static void SetAllowedWorkTypes(IEnumerable<WorkTypeDef> newList)
        {
            allowedWorkTypes = new List<WorkTypeDef>();
            foreach(WorkTypeDef workDef in newList)
            {
                allowedWorkTypes.Add(workDef);
            }
        }

        public static void InitWorkSettings(Pawn pawn)
        {
            //Work Types
            if (!pawn.workSettings.EverWork)
                pawn.workSettings.EnableAndInitialize();
            foreach (WorkTypeDef def in DefDatabase<WorkTypeDef>.AllDefs)
                if(WorkDisabled(def))
                    pawn.workSettings.Disable(def);

            //Timetables
            if(pawn.timetable == null)
                pawn.timetable = new Pawn_TimetableTracker(pawn);

            //Restrict areas
            pawn.playerSettings.AreaRestriction = null;
        }

        public static bool LaborEnabled(Pawn pawn)
        {
            if (pimDef == null)
                pimDef = DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption");
            if (pawn.IsPrisoner && pawn.guest.interactionMode == pimDef && !PrisonLaborPrefs.DisableMod)
                return true;
            else
                return false;
        }

        public static bool WorkTime(Pawn pawn)
        {
            if (pawn.timetable == null)
                return true;
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
                return true;
            if(pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything)
            {
                if (HealthAIUtility.ShouldSeekMedicalRest(pawn) || pawn.needs.food.CurCategory > HungerCategory.Hungry || pawn.needs.rest.CurCategory != RestCategory.Rested)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
