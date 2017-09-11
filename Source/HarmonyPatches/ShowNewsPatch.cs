using System;
using Harmony;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Map))]
    [HarmonyPatch("FinalizeInit")]
    [HarmonyPatch(new Type[] { })]
    internal class ShowNewsPatch
    {
        private static void Postfix()
        {
            NewsDialog.TryShow();
        }
    }
}