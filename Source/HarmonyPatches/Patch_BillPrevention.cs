using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;
using System;
using System.IO;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(WorkGiver_DoBill))]
    [HarmonyPatch("StartOrResumeBillJob")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(IBillGiver) })]
    internal class Patch_BillPrevention
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            OpCode[] opCodes =
            {
                OpCodes.Ldfld,
                OpCodes.Ldfld,
                OpCodes.Beq,
                OpCodes.Br,
            };
            String[] operands =
            {
                "RimWorld.WorkGiverDef def",
                "Verse.WorkTypeDef workType",
                "System.Reflection.Emit.Label",
                "System.Reflection.Emit.Label",
            };
            var label = (Label)HPatcher.FindOperandAfter(opCodes, operands, instr);

            OpCode[] opCodes2 =
            {
                OpCodes.Ldc_I4_0,
                OpCodes.Stloc_0,
                OpCodes.Br,
                OpCodes.Ldarg_2,
                OpCodes.Callvirt,
                OpCodes.Ldloc_0,
                OpCodes.Callvirt,
                OpCodes.Stloc_1,
            };
            String[] operands2 =
            {
                "",
                "",
                "System.Reflection.Emit.Label",
                "",
                "RimWorld.BillStack get_BillStack()",
                "",
                "RimWorld.Bill get_Item(Int32)",
                "",
            };
            var step = 0;
            foreach (var ci in instr)
            {
                yield return ci;
                if (HPatcher.IsFragment(opCodes2, operands2, ci, ref step, "Patch_BillPrevention"))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_BillPrevention).GetMethod("IsForCertainGroup"));
                    yield return new CodeInstruction(OpCodes.Brfalse, label);
                }
            }
        }

        public static bool IsForCertainGroup(Pawn pawn, Bill bill)
        {
            var group = BillUtility.IsFor(bill);
            if (group == GroupMode.ColonyOnly)
                return true;
            if (group == GroupMode.ColonistsOnly && !pawn.IsPrisoner)
                return true;
            if (group == GroupMode.PrisonersOnly && pawn.IsPrisoner)
                return true;
            return false;
        }
    }
}