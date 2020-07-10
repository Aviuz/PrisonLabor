using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_DeliverFood
{
    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood))]
    [HarmonyPatch(nameof(WorkGiver_Warden_DeliverFood.JobOnThing))]
    public class Patch_DeliverFood
    {
        // remove this line        
        // if (!pawn2.Position.IsInPrisonCell(pawn2.Map))
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var mcall = AccessTools.Method("GridsUtility:IsInPrisonCell");
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i + 0].opcode == OpCodes.Callvirt &&
                    codes[i + 1].opcode == OpCodes.Ldloc_0 &&
                    codes[i + 2].opcode == OpCodes.Callvirt &&
                    codes[i + 3].opcode == OpCodes.Call &&
                    codes[i + 4].opcode == OpCodes.Brtrue_S)
                {
                    if (codes[i + 3].operand as MethodInfo == mcall)
                    {
                        i += 7;
                    }
                    else
                    {
                        yield return codes[i];
                    }
                }
                else
                {
                    yield return codes[i];
                }
            }
        }
    }
}
