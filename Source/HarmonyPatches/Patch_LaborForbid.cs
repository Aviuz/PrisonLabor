using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(JobGiver_Work))]
    [HarmonyPatch("TryIssueJobPackage")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(JobIssueParams) })]
    internal class Patch_LaborForbid
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            var pawn = HarmonyPatches.FindOperandAfter(new[] { OpCodes.Ldfld }, new[] { "Verse.Pawn pawn" }, instr);
            var jobgiver = HarmonyPatches.FindOperandAfter(new[] {OpCodes.Ldloc_S }, new[] { "RimWorld.JobGiver_Work+<TryIssueJobPackage>c__AnonStorey2A9 (27)" }, instr );
            var scanner = HarmonyPatches.FindOperandAfter(new[] { OpCodes.Ldfld }, new[] { "RimWorld.WorkGiver_Scanner scanner" }, instr);

            OpCode[] opcodes =
            {
                OpCodes.Ldloc_S,
                OpCodes.Ldftn,
                OpCodes.Newobj,
            };
            String[] operands =
            {
                "RimWorld.JobGiver_Work+<TryIssueJobPackage>c__AnonStorey2A9 (27)",
                "Boolean <>m__14E(Verse.Thing)",
                "Void .ctor(Object, IntPtr)",
            };
            int step = 0;

            foreach (var ci in instr)
            {
                yield return ci;

                if (HarmonyPatches.IsFragment(opcodes, operands, ci, ref step))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldarg_1, null);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, jobgiver);
                    yield return new CodeInstruction(OpCodes.Ldfld, scanner);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_LaborForbid).GetMethod("CreatePredicate"));
                }
            }
        }

        public static Predicate<Thing> CreatePredicate(Pawn pawn, WorkGiver_Scanner scanner)
        {
            return t => !t.IsForbidden(pawn)
                        && scanner.HasJobOnThing(pawn, t, false)
                        && !LaborExclusionUtility.IsDisabledByLabor(t.Position, pawn, scanner.def.workType);
        }
    }
}