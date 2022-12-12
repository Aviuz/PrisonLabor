using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch]
  class Patch_RepairUtility
  {
    static IEnumerable<MethodBase> TargetMethods()
    {
      yield return typeof(RepairUtility).GetMethod(nameof(RepairUtility.PawnCanRepairEver));
      yield return typeof(RepairUtility).GetMethod(nameof(RepairUtility.PawnCanRepairNow));
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      var codes = new List<CodeInstruction>(instructions);
      for (int i = 0; i < codes.Count(); i++)
      {
        if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
        {
          yield return new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod(nameof(PrisonLaborUtility.GetPawnFaction)));
        }
        else
        {
          yield return codes[i];
        }
      }
    }

    private static bool ShouldPatch(CodeInstruction actual, CodeInstruction prev)
    {
      return prev.opcode == OpCodes.Ldarg_0 && actual.opcode == OpCodes.Callvirt && actual.operand != null && actual.operand.ToString().Contains("RimWorld.Faction get_Faction()");
    }
  }

  [HarmonyPatch(typeof(WorkGiver_Repair), "HasJobOnThing")]
  class Patch_WorkGiver_Repair
  {
    static bool Postfix(bool __result, Pawn pawn, Thing t, bool forced)
    {
      if (__result && pawn.IsPrisonerOfColony)
      {
        return __result && pawn.CanReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger());
      }
      return __result;
    }
  }
}
