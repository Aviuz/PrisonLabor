using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using Verse.Noise;
using System.Reflection.Emit;
using PrisonLabor.HarmonyPatches;

namespace KijinCompatibility.HarmonyPatches
{
  [HarmonyPatch]
  class PrisonerHarvestResourcesPatch
  {
    static MethodBase TargetMethod()
    {
      return AccessTools.Method("Kijin3.Kijin3PlantCollectedPatch:GetFirstPawnNotDeadOrDowned");
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
      return prev.opcode == OpCodes.Ldloc_2 && actual.opcode == OpCodes.Callvirt && actual.operand != null && actual.operand.ToString().Contains("RimWorld.Faction get_Faction()");
    }

  }
}
