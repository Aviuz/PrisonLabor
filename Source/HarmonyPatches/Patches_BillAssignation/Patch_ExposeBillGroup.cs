using System;
using Harmony;
using PrisonLabor.Core.BillAssignation;
using PrisonLabor.Core.Meta;
using RimWorld;

namespace PrisonLabor.HarmonyPatches.Patches_BillAssignation
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

            BillAssignationUtility.GetData(__instance).ExposeData();
        }
    }
}