using System;
using Harmony;
using RimWorld;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Bill))]
    [HarmonyPatch("ExposeData")]
    [HarmonyPatch(new Type[] { })]
    internal class Patch_ExposeBillGroup
    {
        private static void Postfix(Bill __instance)
        {
            if (PrisonLaborPrefs.DisableMod)
                return;

            BillUtility.GetData(__instance).ExposeData();
        }
    }
}