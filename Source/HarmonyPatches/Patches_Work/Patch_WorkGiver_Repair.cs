using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Verse;
using Verse.AI;
using PrisonLabor.Core.Other;
using static Verse.HediffCompProperties_RandomizeSeverityPhases;

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

    static IEnumerable<CodeInstruction> Transpiler(MethodBase mBase, IEnumerable<CodeInstruction> instructions)
    {
      var codes = new List<CodeInstruction>(instructions);
      for (int i = 0; i < codes.Count(); i++)
      {
        if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
        {
          DebugLogger.debug($"Patch_RepairUtility patch: {mBase.ReflectedType.Assembly.GetName().Name}.{mBase.ReflectedType.Name}.{mBase.Name}");
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
      return prev.opcode == OpCodes.Ldarg_0 && HPatcher.IsGetFactionOperand(actual);
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
