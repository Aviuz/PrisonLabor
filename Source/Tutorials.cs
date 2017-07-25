using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    class Tutorials
    {
        private static ConceptDef introductionDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Indroduction", true);
        private static ConceptDef motivationDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Motivation", true);
        private static ConceptDef growingDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Growing", true);
        private static ConceptDef managementDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Management", true);
        private static ConceptDef timetableDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Timetable", true);

        public static void Introduction()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(introductionDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(introductionDef);
            //Move it to point after map genration
            NewsDialog.TryShow();
        }

        public static void Motivation()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(motivationDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(motivationDef);
        }

        public static void Management()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(managementDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(managementDef);
        }

        public static void Timetable()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(timetableDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(timetableDef);
        }

        public static void Growing()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(growingDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(growingDef);
        }

    }
}
