using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace PrisonLabor.CompatibilityPatches
{
    static internal class SeedsPlease
    {
        static public void Init()
        {
            if (DefDatabase<JobDef>.GetNamed("SowWithSeeds", false) != null)
            {
                try
                {
                    WorkGiverDef seedsPleaseDef = DefDatabase<WorkGiverDef>.GetNamed("GrowerSow");
                    seedsPleaseDef.giverClass = typeof(SeedsPlease_WorkGiver);
                    JobDef prisonLaborDef = JobDefOf.Harvest;
                    SeedsPlease_WorkDriver_Patch.Run();
                    prisonLaborDef.driverClass = DefDatabase<JobDef>.GetNamed("Harvest").driverClass;
                }
                catch (Exception e)
                {
                    Log.Error("Prison Labor encountered problem with SeedsPlease mod. Failed to patch. Error message:\n" + e.ToString());
                }
            }
        }

        static public bool CanOverrideHarvest()
        {
            return DefDatabase<JobDef>.GetNamed("SowWithSeeds", false) == null;
        }
    }
}
