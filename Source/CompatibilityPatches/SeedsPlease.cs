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
        static internal void Init()
        {
            if (Check())
                Work();
        }

        static internal bool Check()
        {
            if (DefDatabase<JobDef>.GetNamed("SowWithSeeds", false) != null)
                return true;
            else
                return false;
        }

        static internal void Work()
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
                    Log.Error("PrisonLaborException: encountered problem with SeedsPlease mod. Failed to patch:\n" + e.ToString());
                }
        }

        static internal bool CanOverrideHarvest()
        {
            return DefDatabase<JobDef>.GetNamed("SowWithSeeds", false) == null;
        }
    }
}
