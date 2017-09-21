using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(WorkGiver_DoBill))]
    [HarmonyPatch("StartOrResumeBillJob")]
    [HarmonyPatch(new[] {typeof(Pawn), typeof(IBillGiver)})]
    internal class Patch_BillPrevention
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            var label = gen.DefineLabel();
            var step = 0;

            foreach (var instruction in instr)
            {
                if (step == 1)
                {
                    instruction.labels.Add(label);
                    step++;
                }
                yield return instruction;
                if (step == 0 && instruction.opcode == OpCodes.Stloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call,
                        typeof(Patch_BillPrevention).GetMethod("IsForCertainGroup"));
                    yield return new CodeInstruction(OpCodes.Brtrue, label);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Ret);
                    step++;
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