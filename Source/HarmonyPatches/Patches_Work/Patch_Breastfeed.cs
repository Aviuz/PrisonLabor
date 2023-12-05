using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
      if (mom.IsPrisonerOfColony || baby.IsPrisonerOfColony)
      {
        return ChildcareUtility.HasBreastfeedCompatibleFactions(PrisonLaborUtility.GetPawnFaction(mom), baby);
      }
      return __result;
    }

    [HarmonyPatch("HasBreastfeedCompatibleFactions")]
    [HarmonyPatch(new[] { typeof(Faction), typeof(Pawn)})]
    public static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> inst)
    {
      var codes = new List<CodeInstruction>(inst);
      for (int i = 0; i < codes.Count(); i++)
      {
        if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
        {
          DebugLogger.debug($"Patch_WorkGiver_PrisonerFaction patch: {mBase.ReflectedType.Assembly.GetName().Name}.{mBase.ReflectedType.Name}.{mBase.Name}");
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
      return prev.opcode == OpCodes.Ldarg_1 && actual.opcode == OpCodes.Callvirt && actual.operand != null && actual.operand.ToString().Contains("RimWorld.Faction get_Faction()");
    }
  }
}
