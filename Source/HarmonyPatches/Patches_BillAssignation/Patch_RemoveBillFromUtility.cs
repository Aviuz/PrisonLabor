using HarmonyLib;
using PrisonLabor.Core.BillAssignation;
using RimWorld;

namespace PrisonLabor.HarmonyPatches.Patches_BillAssignation
{
    [HarmonyPatch(typeof(BillStack))]
    [HarmonyPatch("Delete")]
    [HarmonyPatch(new[] {typeof(Bill)})]
    internal class Patch_RemoveBillFromUtility
    {
        public static void Postfix(Bill bill)
        {
            BillAssignationUtility.Remove(bill);
        }
    }
}