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

        public static bool showNews = true;
        public static bool msgShowVersion0_5 = false;
        public static bool msgShowVersion0_6 = false;

        public static void Introduction()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(introductionDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(introductionDef);
            //Move it to point after map genration
            News();
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

        public static void News()
        {
            if (showNews)
            {
                if (msgShowVersion0_6)
                {
                    Find.WindowStack.Add(new Dialog_MessageBox("Changes in PrisonLabor v0.6:\n\n   1. Time restrictions - now you can manage your prisoners time for sleep, work and joy. You can now even force them to work when they're hungry!\n   2. Getting food by prisoners - Now prisoners will look for food in much better way, and now (when they desperate enough) they will eat corpses!\n   3. \"Laziness\" changed to \"Motivation\" and inverted.\n\n   ATTENTION: After PrisonLabor reaches beta all saves with PrisonLabor v0.5a or lower will be corrupted and unplayable. This version (0.6) is safe and converts all older saves.", "Ok", null, null, null, "PrisonLabor - Patch 0.6", false));
                }
                if (msgShowVersion0_5)
                {
                    Find.WindowStack.Add(new Dialog_MessageBox("Major changes to PrisonLabor:\n\n   1. Prisoners can now grow, but only plants that not require any skills.\n   2. You can now manage prisoners work types. Just check \"Work\" tab!\n   3. Laziness now appear on \"Needs\" tab. Above 50% wardens will watch prisoners. Above 80% prisoners won't work unless supervised.\n   4. Wardens will now bring food to prisoners that went too far from his bed.\n   5. Prisoners won't gain laziness when not working anymore.\n   6. Fixed many bugs", "Ok", null, null, null, "PrisonLabor - Update", false));
                }
                PrisonLaborPrefs.LastVersion = PrisonLaborPrefs.Version;
                PrisonLaborPrefs.Save();
                showNews = false;
            }
        }

    }
}
