using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_BillAssignation
{
    [HarmonyPatch(typeof(Bill_ProductionWithUft), "get_BoundWorker")]
    class Patch_Bill_ProductionWithUft
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            OpCode[] opCodesToFind =
            {
                OpCodes.Ldloc_0,
                OpCodes.Callvirt,
                OpCodes.Brtrue_S
            };
            string[] operandsToFind =
            {
                "",
                "RimWorld.Faction get_HostFaction()",
                "System.Reflection.Emit.Label"
            };
            Label label = (Label)HPatcher.FindOperandAfter(opCodesToFind, operandsToFind, instr);
            CodeInstruction[] replacemnt =
            {
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Nop, label),
            };
            return HPatcher.ReplaceFragment(opCodesToFind, operandsToFind, instr, replacemnt, "Patch_Bill_ProductionWithUft");
        }
    }
}
