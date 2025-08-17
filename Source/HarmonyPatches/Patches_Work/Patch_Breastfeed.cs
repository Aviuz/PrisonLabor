using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch(typeof(ChildcareUtility))]
  [HarmonyPatch(new[] { typeof(Pawn), typeof(Pawn) })]
  class Patch_BreastfeedCompatibleFactions
  {
    [HarmonyPatch("HasBreastfeedCompatibleFactions")]
    static bool Postfix(bool __result, Pawn mom, Pawn baby)
    {
      if (__result)
      {
        return true;
      }
      return mom.IsPrisonerOfColony && ChildcareUtility.HasBreastfeedCompatibleFactions(PrisonLaborUtility.GetPawnFaction(mom), baby);
    }
  }
}
