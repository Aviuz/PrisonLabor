using HarmonyLib;
using PrisonLabor.Core.BillAssignation;
using PrisonLabor.Core.Other;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Arrest
{
  [HarmonyPatch(typeof(GenAI))]
  [HarmonyPatch(nameof(GenAI.CanBeArrestedBy))]
  public class Patch_ArrestLogic
  {
    private static bool Postfix(bool __result, Pawn pawn, Pawn arrester)
    {
      return __result || pawn.IsPrisonerOfColony && ArrestUtility.IsPawnFleeing(pawn);
    }
  }
}