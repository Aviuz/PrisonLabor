using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker))]
    [HarmonyPatch("ShouldHaveNeed")]
    [HarmonyPatch(new[] {typeof(NeedDef)})]
    internal class NeedOnlyByPrisonersPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            //Searches for loadFieldPawn Instruction. Can't create this by generator (don't know why)
            CodeInstruction loadFieldPawn = null;
            foreach (var ci in instr)
                if (ci.opcode.Value == OpCodes.Ldfld.Value)
                {
                    loadFieldPawn = ci;
                    break;
                }

            // Define label to the begining of the original code
            var jumpTo = gen.DefineLabel();
            //Load argument onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            //Load pawn onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return loadFieldPawn;
            //Call function
            yield return new CodeInstruction(OpCodes.Call,
                typeof(NeedOnlyByPrisonersPatch).GetMethod("ShouldHaveNeedPrisoner"));
            //If true continue
            yield return new CodeInstruction(OpCodes.Brtrue, jumpTo);
            //Load false to stack
            yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            //Return
            yield return new CodeInstruction(OpCodes.Ret);

            var first = true;
            foreach (var ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                //debug
                //Log.Message("CODE: ToString():" + ci.ToString() + " || labels:" + ci.labels.Any());
                yield return ci;
            }
        }


        public static bool ShouldHaveNeedPrisoner(NeedDef nd, Pawn pawn)
        {
            //delete later
            if (nd.defName == "PrisonLabor_Laziness" || nd is Need_Laziness)
                return false;
            if (nd.defName == "PrisonLabor_Motivation" &&
                !(pawn.IsPrisoner && PrisonLaborPrefs.EnableMotivationMechanics))
                return false;
            return true;
        }
    }
}

namespace PrisonLabor.Harmony
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker))]
    [HarmonyPatch("ShouldHaveNeed")]
    [HarmonyPatch(new[] {typeof(NeedDef)})]
    internal class NeedOnlyByPrisonersPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            //Searches for loadFieldPawn Instruction. Can't create this by generator (don't know why)
            CodeInstruction loadFieldPawn = null;
            foreach (var ci in instr)
                if (ci.opcode.Value == OpCodes.Ldfld.Value)
                {
                    loadFieldPawn = ci;
                    break;
                }

            // Define label to the begining of the original code
            var jumpTo = gen.DefineLabel();
            //Load argument onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            //Load pawn onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return loadFieldPawn;
            //Call function
            yield return new CodeInstruction(OpCodes.Call,
                typeof(NeedOnlyByPrisonersPatch).GetMethod("ShouldHaveNeedPrisoner"));
            //If true continue
            yield return new CodeInstruction(OpCodes.Brtrue, jumpTo);
            //Load false to stack
            yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            //Return
            yield return new CodeInstruction(OpCodes.Ret);

            var first = true;
            foreach (var ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                //debug
                //Log.Message("CODE: ToString():" + ci.ToString() + " || labels:" + ci.labels.Any());
                yield return ci;
            }
        }


        public static bool ShouldHaveNeedPrisoner(NeedDef nd, Pawn pawn)
        {
            //delete later
            if (nd.defName == "PrisonLabor_Laziness" || nd is Need_Laziness)
                return false;
            if (nd.defName == "PrisonLabor_Motivation" &&
                !(pawn.IsPrisoner && PrisonLaborPrefs.EnableMotivationMechanics))
                return false;
            return true;
        }
    }
}