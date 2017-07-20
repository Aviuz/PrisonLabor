using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    class WorkAssignmentsUtility
    {

        private static List<WorkTypeDef> disabledWorks;

        public static List<WorkTypeDef> DisabledWorks
        {
            get
            {
                if(disabledWorks == null)
                {
                    disabledWorks = new List<WorkTypeDef>();
                    disabledWorks.Add(WorkTypeDefOf.Construction);
                    disabledWorks.Add(WorkTypeDefOf.Doctor);
                    disabledWorks.Add(WorkTypeDefOf.Firefighter);
                    disabledWorks.Add(WorkTypeDefOf.Handling);
                    disabledWorks.Add(WorkTypeDefOf.Hunting);
                    disabledWorks.Add(WorkTypeDefOf.Warden);
                    disabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("Art"));
                    disabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("PatientEmergency"));
                    disabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("PatientBedRest"));
                    disabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("Flicker"));
                    disabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("Research"));
                    disabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("Smithing"));
                    disabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("Tailoring"));
                }
                return disabledWorks;
            }
        }

        public static bool Disabled(WorkTypeDef wt)
        {
            return DisabledWorks.Contains(wt);
        }

        public static bool Disabled(Pawn p, WorkTypeDef wt)
        {
            if (p.IsPrisonerOfColony)
                return DisabledWorks.Contains(wt);
            else
                return false;
        }

        public static void initWorkSettings(Pawn pawn)
        {
            //Work Types
            if (!pawn.workSettings.EverWork)
                pawn.workSettings.EnableAndInitialize();
            foreach (WorkTypeDef def in WorkAssignmentsUtility.DisabledWorks)
                pawn.workSettings.Disable(def);

            //Timetables
            if(pawn.timetable == null)
                pawn.timetable = new Pawn_TimetableTracker(pawn);

            //Restrict areas
            pawn.playerSettings.AreaRestriction = null;
        }
    }
}
