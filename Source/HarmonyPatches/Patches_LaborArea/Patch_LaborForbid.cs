using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_LaborArea
{
/*    [HarmonyPatch(typeof(JobGiver_Work))]
    [HarmonyPatch("TryIssueJobPackage")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(JobIssueParams) })]
    internal class Patch_LaborForbid
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            // Temporary class for main loop
            OpCode[] opCodesClass =
{
                OpCodes.Ldloc_S,
            };
            string[] operandsClass =
            {
                "RimWorld.JobGiver_Work+<>c__DisplayClass3_1 (9)",
            };
            var tempClass = HPatcher.FindOperandAfter(opCodesClass, operandsClass, instr, true);

            // Scanner
            OpCode[] opCodesScanner =
{
                OpCodes.Ldfld,
            };
            string[] operandsScanner =
            {
                "RimWorld.WorkGiver_Scanner scanner",
            };
            var scanner = HPatcher.FindOperandAfter(opCodesScanner, operandsScanner, instr, true);

            // Fragment where cells are collected as IEnumerable
            OpCode[] opCodesFunc =
            {
                OpCodes.Callvirt,
            };
            string[] operandsFunc =
            {
                "System.Collections.Generic.IEnumerable`1[Verse.IntVec3] PotentialWorkCellsGlobal(Verse.Pawn)",
            };
            int stepFunc = 0;

            // Transpiler
            foreach (var instruction in instr)
            {
                yield return instruction;
                if (HPatcher.IsFragment(opCodesFunc, operandsFunc, instruction, ref stepFunc, "LaborForbid", true))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, tempClass);
                    yield return new CodeInstruction(OpCodes.Ldfld, scanner);
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(WorkGiver).GetField("def"));
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(WorkGiverDef).GetField("workType"));
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_LaborForbid).GetMethod("RemoveFromListForbiddenCells"));
                }
            }
        }

        public static IEnumerable<IntVec3> RemoveFromListForbiddenCells(IEnumerable<IntVec3> list, Pawn pawn, WorkTypeDef workType)
        {
            Log.Message($"Checking work type: {workType.defName}, list size: {list.Count()}");
            IEnumerable<IntVec3> result = list.Where(i => !PrisonLaborUtility.IsDisabledByLabor(i, pawn, workType));
            Log.Message($"After filtration work type: {workType.defName}, list size: {result.Count()}");
            return result;
        }
    }*/
}
