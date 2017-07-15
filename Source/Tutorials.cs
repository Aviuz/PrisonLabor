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

        public static void PrisonLabor()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(prisonLaborDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(prisonLaborDef);
            //Move it to point after map genration
            if(Initialization.oldPlayerNotification)
            {
                Find.WindowStack.Add(new Dialog_MessageBox("PrisonLabor machanics has changed.\n\n   1. Prisoner Interaction mode must be set to \"Work\" instead of \"No Interaction\".\n   2. Now prisoners will get lazy if not supervised. Warden job will include watching prisoners to prevent that (you can always draft your colonists).", "Ok", null, null, null, "PrisonLabor - New mechanics", false));
                Initialization.oldPlayerNotification = false;
            }
        }

        public static void LazyPrisoner()
        {
            if (!PlayerKnowledgeDatabase.IsComplete(lazyPrisonerDef))
                Verse.Find.Tutor.learningReadout.TryActivateConcept(lazyPrisonerDef);
        }

    }
}
