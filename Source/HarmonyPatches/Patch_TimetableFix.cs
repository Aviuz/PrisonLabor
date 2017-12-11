using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using System.Reflection.Emit;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn_TimetableTracker))]
    [HarmonyPatch("get_CurrentAssignment")]
    internal class Patch_TimetableFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            var pawn = HPatcher.FindOperandAfter(new[] { OpCodes.Ldfld }, new[] { "Verse.Pawn pawn" }, instr);
            var label = HPatcher.FindOperandAfter(new[] { OpCodes.Brtrue }, new[] { "System.Reflection.Emit.Label" }, instr);

            OpCode[] opCodes =
            {
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Callvirt,
                OpCodes.Brtrue,
            };
            string[] operands =
            {
                "",
                "Verse.Pawn pawn",
                "Boolean get_IsColonist()",
                "System.Reflection.Emit.Label",
            };
            int step = 0;

            foreach (var ci in instr)
            {
                yield return ci;

                if(HPatcher.IsFragment(opCodes, operands, ci, ref step, "Patch_TimetableFix"))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, pawn);
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(Pawn).GetMethod("get_IsPrisonerOfColony"));
                    yield return new CodeInstruction(OpCodes.Brtrue, label);
                }
            }
        }
    }
}
