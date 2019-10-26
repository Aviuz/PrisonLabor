using PrisonLabor.Constants;
using PrisonLabor.Core.LaborWorkSettings;
using PrisonLabor.Core.Meta;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.CompatibilityPatches
{
    internal static class OlderVersions
    {
        internal static void Pre_v0_9_4()
        {
            if (WorkSettings.AllowedWorkTypes.Contains(WorkTypeDefOf.Warden))
                WorkSettings.AllowedWorkTypes.Remove(WorkTypeDefOf.Warden);
            if (WorkSettings.AllowedWorkTypes.Contains(PL_DefOf.PrisonLabor_Jailor))
                WorkSettings.AllowedWorkTypes.Remove(PL_DefOf.PrisonLabor_Jailor);

            WorkSettings.Apply();
            PrisonLaborPrefs.Save();
        }
    }
}
