using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches
{

    [HarmonyPatch(typeof(Map))]
    [HarmonyPatch(nameof(Map.MapPostTick))]
    static class Patch_InsiprationTracker
    {
        static bool Prefix(Map __instance)
        {
            InspirationTracker.Calculate(__instance);
            return true;
        }
    }
}
