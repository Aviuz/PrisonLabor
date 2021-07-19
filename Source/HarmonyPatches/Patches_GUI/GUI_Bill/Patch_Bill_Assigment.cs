using HarmonyLib;
using PrisonLabor.Core.BillAssignation;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_Bill
{
    [HarmonyPatch(typeof(Bill))]
    public class Patch_Bill_Assigment
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetAnyPawnRestriction")]
        static void ColonistPostFix(Bill __instance)
        {
            BillAssignationUtility.SetFor(__instance, GroupMode.ColonistsOnly);
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetAnySlaveRestriction")]
        static void SlavePostFix(Bill __instance)
        {
            BillAssignationUtility.SetFor(__instance, GroupMode.SlavesOnly);
        }
    }
}
