using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Restraints
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("TicksPerMove")]
    class Patch_RestrainsPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            OpCode[] opCodes =
            {
                OpCodes.Call,
                OpCodes.Brfalse,
            };
            String[] operands =
            {
                "Boolean InRestraints(Verse.Pawn)",
                "System.Reflection.Emit.Label",
            };
            int step = 0;

            foreach(var ci in instr)
            {
                if (HPatcher.IsFragment(opCodes, operands, ci, ref step, "Patch_RestrainsPatch"))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br, ci.operand);
                }
                else
                    yield return ci;
            }
        }
    }
}
