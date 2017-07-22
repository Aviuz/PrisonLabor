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

        private static List<WorkTypeDef> enabledWorks;
        private static List<WorkTypeDef> disabledWorks;

        public static List<WorkTypeDef> DisabledWorks
        {
            get
            {
                if(enabledWorks == null)
                {
                    enabledWorks = new List<WorkTypeDef>();
                    enabledWorks.Add(WorkTypeDefOf.Growing);
                    enabledWorks.Add(WorkTypeDefOf.Mining);
                    enabledWorks.Add(WorkTypeDefOf.Hauling);
                    enabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("Cooking"));
                    enabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("PlantCutting"));
                    enabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("Crafting"));
                    enabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("Cleaning"));

                    enabledWorks.Add(DefDatabase<WorkTypeDef>.GetNamed("HaulingUrgent", false));
                }
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

        public static bool WorkDisabled(WorkTypeDef wt)
        {
            if (wt != null)
                return DisabledWorks.Contains(wt);
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

        public static void InitWorkSettings(Pawn pawn)
        {
            //Work Types
            if (!pawn.workSettings.EverWork)
                pawn.workSettings.EnableAndInitialize();
            foreach (WorkTypeDef def in PrisonLaborUtility.DisabledWorks)
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
            if (pawn.IsPrisoner && pawn.guest.interactionMode == pimDef)
                return true;
            else
                return false;
        }
    }
}
