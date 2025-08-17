using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch(typeof(HaulAIUtility), nameof(HaulAIUtility.HaulToStorageJob))]
  public class Patch_Haul
  {
    public static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
      IEnumerable<CodeInstruction> inst)
    {
      var codes = new List<CodeInstruction>(inst);
      for (int i = 0; i < codes.Count; i++)
      {
        if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
        {
          DebugLogger.debug(
            $"Patch_Haul patch: {mBase.ReflectedType.Assembly.GetName().Name}.{mBase.ReflectedType.Name}.{mBase.Name}");
          yield return new CodeInstruction(OpCodes.Call,
            typeof(PrisonLaborUtility).GetMethod(nameof(PrisonLaborUtility.GetPawnFaction)));
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
}