using HarmonyLib;
using PrisonLabor.Core.Other;
using PrisonLabor.HarmonyPatches.Patches_Work;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.CompatibilityPatches
{
    static class CleaningArea
    {
        internal static void Init()
        {
            ModSearcher modSeeker = new ModSearcher("CleaningArea");
            if (modSeeker.LookForMod())
            {
                Patch();
            }
        }

        private static void Patch()
        {
            try
            {
                MethodBase methodBase = getTargetMethod();
                if (methodBase != null)
                {
                    var harmony = new Harmony("Harmony_PrisonLabor_CleaningArea");
                    harmony.Patch(methodBase, transpiler: new HarmonyMethod(typeof(Patch_WorkGiver_PrisonerFaction).GetMethod("Transpiler")));
                }
            }
            catch (Exception e)
            {
                Log.Error($"PrisonLaborException: failed to proceed CleaningArea mod patches: {e.ToString()}");
            }

        }

        private static MethodBase getTargetMethod()
        {
            return AccessTools.Method("CleaningArea.WorkGiver_CleanFilth_CleaningArea:HasJobOnThing");
        }
    }
}
