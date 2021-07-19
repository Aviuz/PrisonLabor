using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Tweaks
{
    public static class ClassInjector
    {
        public static void Init()
        {
            SplitWardenType();
        }

        private static void SplitWardenType()
        {
            DefDatabase<WorkGiverDef>.GetNamed("DoExecution").workType = PL_DefOf.PrisonLabor_Jailor;
            DefDatabase<WorkGiverDef>.GetNamed("ReleasePrisoner").workType = PL_DefOf.PrisonLabor_Jailor;
            DefDatabase<WorkGiverDef>.GetNamed("TakePrisonerToBed").workType = PL_DefOf.PrisonLabor_Jailor;
            //DefDatabase<WorkGiverDef>.GetNamed("FeedPrisoner").workType = PrisonLaborDefOf.PrisonLabor_Jailor;
            //DefDatabase<WorkGiverDef>.GetNamed("DeliverFoodToPrisoner").workType = PrisonLaborDefOf.PrisonLabor_Jailor;
            WorkTypeDefOf.Warden.workGiversByPriority.Clear();
            WorkTypeDefOf.Warden.ResolveReferences();
            PL_DefOf.PrisonLabor_Jailor.workGiversByPriority.Clear();
            PL_DefOf.PrisonLabor_Jailor.ResolveReferences();
        }
    }
}
