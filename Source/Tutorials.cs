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
        private static ConceptDef prisonLaborDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor", true);
        private static ConceptDef lazyPrisonerDef = DefDatabase<ConceptDef>.GetNamed("LazyPrisoner", true);
        public static bool msgShowVersion0_3 = false;
        public static bool msgShowVersion0_5 = false;

        public static void PrisonLabor()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(prisonLaborDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(prisonLaborDef);
            //Move it to point after map genration
            News();
        }

        public static void LazyPrisoner()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(lazyPrisonerDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(lazyPrisonerDef);
        }

        public static void News()
        {
            if (msgShowVersion0_5)
            {
                Find.WindowStack.Add(new Dialog_MessageBox("Major changes to PrisonLabor:\n\n   1. Prisoners can now grow, but only plants that not require any skills.\n   2. You can now manage prisoners work types. Just check \"Work\" tab!\n   3. Laziness now appear on \"Needs\" tab. Above 50% wardens will watch prisoners. Above 80% prisoners won't work unless supervised.\n   4. Wardens will now bring food to prisoners that went too far from his bed.\n   5. Prisoners won't gain laziness when not working anymore.\n   6. Fixed many bugs", "Ok", null, null, null, "PrisonLabor - Update", false));
                msgShowVersion0_5 = false;
            }
            if (msgShowVersion0_3)
            {
                Find.WindowStack.Add(new Dialog_MessageBox("PrisonLabor machanics has changed.\n\n   1. Prisoner Interaction mode must be set to \"Work\" instead of \"No Interaction\".\n   2. Now prisoners will get lazy if not supervised. Warden job will include watching prisoners to prevent that (you can always draft your colonists).", "Ok", null, null, null, "PrisonLabor - New mechanics", false));
                msgShowVersion0_3 = false;
            }
        }

    }
}
