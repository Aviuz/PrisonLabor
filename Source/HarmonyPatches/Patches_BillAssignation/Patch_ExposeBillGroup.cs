using System;
using HarmonyLib;
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
            BillAssignationUtility.GetData(__instance).ExposeData();
        }
    }
}