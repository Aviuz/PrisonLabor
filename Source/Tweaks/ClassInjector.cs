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
            UITweaks();
            SplitWardenType();
        }

        private static void UITweaks()
        {
            // Replace work tab with custom one
            var workTab = DefDatabase<MainButtonDef>.GetNamed("Work");
            MainTabWindow_Work_Tweak.MainTabWindowType = workTab.tabWindowClass;
            workTab.tabWindowClass = typeof(MainTabWindow_Work_Tweak);

            // Replace assign tab with custom one
            var assignTab = DefDatabase<MainButtonDef>.GetNamed("Restrict");
            MainTabWindow_Restrict_Tweak.MainTabWindowType = assignTab.tabWindowClass;
            assignTab.tabWindowClass = typeof(MainTabWindow_Restrict_Tweak);
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
