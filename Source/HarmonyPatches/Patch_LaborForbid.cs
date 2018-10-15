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
            //var pawn = HPatcher.FindOperandAfter(new[] { OpCodes.Ldfld }, new[] { "Verse.Pawn pawn" }, instr);
            var jobgiver = HPatcher.FindOperandAfter(new[] {OpCodes.Ldloc_S }, new[] { "RimWorld.JobGiver_Work+<TryIssueJobPackage>c__AnonStorey1 (11)" }, instr );
            var scanner = HPatcher.FindOperandAfter(new[] { OpCodes.Ldfld }, new[] { "RimWorld.WorkGiver_Scanner scanner" }, instr);
            var cell = HPatcher.FindOperandAfter(new[] {OpCodes.Ldloc_S }, new[] { "Verse.IntVec3 (33)" }, instr );

            OpCode[] opcodes1 =
            {
                OpCodes.Ldloc_S,
                OpCodes.Ldftn,
                OpCodes.Newobj,
            };
            String[] operands1 =
            {
                "RimWorld.JobGiver_Work+<TryIssueJobPackage>c__AnonStorey1 (11)",
                "Boolean <>m__0(Verse.Thing)",
                "Void .ctor(Object, IntPtr)",
            };
            int step1 = 0;

            // Find else if with ..  && !current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current, false) and add && IsDisabledByLabor()
            OpCode[] opCodes2 =
           {
                OpCodes.Bge_Un,
                OpCodes.Ldloc_S,
                OpCodes.Ldloc_0,
                OpCodes.Ldfld,
                OpCodes.Call,
                OpCodes.Brtrue,
                OpCodes.Ldloc_S,
                OpCodes.Ldfld,
                OpCodes.Ldloc_0,
                OpCodes.Ldfld,
                OpCodes.Ldloc_S,
                OpCodes.Ldc_I4_0,
                OpCodes.Callvirt,
                OpCodes.Brfalse,
            };
            String[] operands2 =
            {
                "System.Reflection.Emit.Label",
                "Verse.IntVec3 (33)",
                "",
                "Verse.Pawn pawn",
                "Boolean IsForbidden(IntVec3, Verse.Pawn)",
                "System.Reflection.Emit.Label",
                "RimWorld.JobGiver_Work+<TryIssueJobPackage>c__AnonStorey1 (11)",
                "RimWorld.WorkGiver_Scanner scanner",
                "",
                "Verse.Pawn pawn",
                "Verse.IntVec3 (33)",
                "",
                "Boolean HasJobOnCell(Verse.Pawn, IntVec3, Boolean)",
                "System.Reflection.Emit.Label",
            };
            int step2 = 0;

            foreach (var ci in instr)
            {
                yield return ci;

                if (HPatcher.IsFragment(opcodes1, operands1, ci, ref step1, "Patch_LaborForbid1"))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, jobgiver);
                    yield return new CodeInstruction(OpCodes.Ldfld, scanner);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_LaborForbid).GetMethod("CreatePredicate"));
                }

                if(HPatcher.IsFragment(opCodes2, operands2, ci, ref step2, "Patch_LaborForbid2"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, cell);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, jobgiver);
                    yield return new CodeInstruction(OpCodes.Ldfld, scanner);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_LaborForbid).GetMethod("GetWorkType"));
                    yield return new CodeInstruction(OpCodes.Call, typeof(LaborExclusionUtility).GetMethod("IsDisabledByLabor"));
                    yield return new CodeInstruction(OpCodes.Brtrue, ci.operand);
                }
            }
        }

        public static Predicate<Thing> CreatePredicate(Pawn pawn, WorkGiver_Scanner scanner)
        {
            return t => !t.IsForbidden(pawn)
                        && scanner.HasJobOnThing(pawn, t, false)
                        && !LaborExclusionUtility.IsDisabledByLabor(t.Position, pawn, scanner.def.workType);
        }

        public static WorkTypeDef GetWorkType(WorkGiver_Scanner scanner)
        {
            return scanner.def.workType;
        }
    }
}
 