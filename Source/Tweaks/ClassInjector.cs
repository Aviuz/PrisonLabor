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
            // Deliver food to prisoners (include other rooms etc.)
            var deliverFoodWorkGiver = DefDatabase<WorkGiverDef>.GetNamed("DeliverFoodToPrisoner");
            deliverFoodWorkGiver.giverClass = typeof(WorkGiver_Warden_DeliverFood_Tweak);

            // Mine
            var minerJob = JobDefOf.Mine;
            minerJob.driverClass = typeof(JobDriver_Mine_Tweak);

            // Cut plant
            var cutPlantJob = JobDefOf.CutPlant;
            cutPlantJob.driverClass = typeof(JobDriver_PlantCut_Tweak);

            // Harvest
            var harvestJob = JobDefOf.Harvest;
            harvestJob.driverClass = typeof(JobDriver_PlantHarvest_Tweak);

            // Grow
            var growWorkGiver = DefDatabase<WorkGiverDef>.GetNamed("GrowerSow");
            growWorkGiver.giverClass = typeof(WorkGiver_GrowerSow_Tweak);

            // Clean
            var cleanWorkGiver = DefDatabase<WorkGiverDef>.GetNamed("CleanFilth");
            cleanWorkGiver.giverClass = typeof(WorkGiver_CleanFilth_Tweak);
        }
    }
}
