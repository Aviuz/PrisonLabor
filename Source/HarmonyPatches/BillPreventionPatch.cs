using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(WorkGiver_DoBill))]
    [HarmonyPatch("StartOrResumeBillJob")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(IBillGiver) })]
    class BillPreventionPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            Label label = gen.DefineLabel();
            int step = 0;

            foreach(CodeInstruction instruction in instr)
            {
                if(step == 1)
                {
                    instruction.labels.Add(label);
                    step++;
                }
                yield return (instruction);
                if(step == 0 && instruction.opcode == OpCodes.Stloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(BillPreventionPatch).GetMethod("IsForCertainGroup"));
                    yield return new CodeInstruction(OpCodes.Brtrue, label);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Ret);
                    step++;
                }
            }
        }

        public static bool IsForCertainGroup(Pawn pawn, Bill bill)
        {
            GroupMode group = BillUtility.IsFor(bill);
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
