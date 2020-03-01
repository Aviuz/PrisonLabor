using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PrisonLabor.Core.Meta;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Needs
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker))]
    [HarmonyPatch("ShouldHaveNeed")]
    [HarmonyPatch(new[] { typeof(NeedDef) })]
    public class Patch_NeedOnlyByPrisoners
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            //Searches for pawn
            var pawn = HPatcher.FindOperandAfter(new[] { OpCodes.Ldfld }, new[] { "Verse.Pawn pawn" }, instr);
            // Define label to the begining of the original code
            var jumpTo = gen.DefineLabel();
            //Load argument onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            //Load pawn onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, pawn);
            //Call function
            yield return new CodeInstruction(OpCodes.Call, typeof(Patch_NeedOnlyByPrisoners).GetMethod(nameof(ShouldHaveNeedPrisoner)));
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
                yield return ci;
            }
        }

        public static bool ShouldHaveNeedPrisoner(NeedDef nd, Pawn pawn)
        {
            if ((nd.defName == "PrisonLabor_Motivation" || nd.defName == "PrisonLabor_Treatment") && !(pawn.IsPrisoner && PrisonLaborPrefs.EnableMotivationMechanics))
                return false;
            return true;
        }
    }
}