using PrisonLabor.Constants;
using PrisonLabor.Core.Meta;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Core.LaborWorkSettings
{
    public static class WorkSettings
    {
        private static List<WorkTypeDef> _availableWorkTypes;
        private static List<WorkTypeDef> _defaultWorkTypes;
        private static List<WorkTypeDef> _allowedWorkTypes;

        public static List<WorkTypeDef> AvailableWorkTypes
        {
            get
            {
                if (_availableWorkTypes == null)
                {
                    _availableWorkTypes = new List<WorkTypeDef>();
                    foreach (var worktype in DefDatabase<WorkTypeDef>.AllDefs)
                        _availableWorkTypes.Add(worktype);

                    _availableWorkTypes.Remove(DefDatabase<WorkTypeDef>.GetNamed("Warden"));
                    _availableWorkTypes.Remove(PL_DefOf.PrisonLabor_Jailor);
                }
                return _availableWorkTypes;
            }
        }

        private static List<WorkTypeDef> DefaultWorkTypes
        {
            get
            {
                if (_defaultWorkTypes == null)
                {
                    _defaultWorkTypes = new List<WorkTypeDef>();
                    _defaultWorkTypes.Add(WorkTypeDefOf.Growing);
                    _defaultWorkTypes.Add(WorkTypeDefOf.Mining);
                    _defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("Hauling"));
                    _defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("Cooking"));
                    _defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("PlantCutting"));
                    _defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("Crafting"));
                    _defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("Cleaning"));

                    _defaultWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed("HaulingUrgent", false));
                }
                return _defaultWorkTypes;
            }
        }

        public static List<WorkTypeDef> AllowedWorkTypes
        {
            get
            {
                if (_allowedWorkTypes == null)
                    return DefaultWorkTypes;
                return _allowedWorkTypes;
            }
            set
            {
                _allowedWorkTypes = value;
                Apply();
            }
        }

        public static string DataString
        {
            get
            {
                if (_allowedWorkTypes == null)
                    return "";
                string data = "";
                foreach (var workDef in _allowedWorkTypes)
                    data += workDef.defName + ";";
                return data;
            }

            set
            {
                if (value.NullOrEmpty())
                {
                    _allowedWorkTypes = null;
                }
                else
                {
                    _allowedWorkTypes = new List<WorkTypeDef>();
                    var subs = value.Split(';');
                    foreach (var s in subs)
                        if (DefDatabase<WorkTypeDef>.GetNamed(s, false) != null)
                            _allowedWorkTypes.Add(DefDatabase<WorkTypeDef>.GetNamed(s));
                }
            }
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
            //pawn.playerSettings.AreaRestriction = null;
        }

        public static bool WorkDisabled(WorkTypeDef wt)
        {
            if (wt != null && PrisonLaborPrefs.AllowAllWorkTypes && AvailableWorkTypes.Contains(wt))
                return false;
            else if (wt != null)
                return !AllowedWorkTypes.Contains(wt);
            else
                return false;
        }

        public static bool WorkDisabled(Pawn p, WorkTypeDef wt)
        {
            if (p.IsPrisoner)
                return WorkDisabled(wt);
            return false;
        }

        public static void Apply()
        {
            PrisonLaborPrefs.AllowedWorkTypes = DataString;
        }
    }
}
