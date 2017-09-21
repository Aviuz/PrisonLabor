using System;
using Harmony;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Map))]
    [HarmonyPatch("FinalizeInit")]
    [HarmonyPatch(new Type[] { })]
    internal class Patch_ShowNews
    {
        private static void Postfix()
        {
            NewsDialog.TryShow();
        }
    }
}