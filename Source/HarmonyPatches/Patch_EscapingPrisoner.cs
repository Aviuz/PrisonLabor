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
    [HarmonyPatch(typeof(JobGiver_PrisonerEscape))]
    [HarmonyPatch("TryGiveJob")]
    [HarmonyPatch(new[] { typeof(Pawn) })]
    public class Patch_EscapingPrisoner
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            Label restOfMethod = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, typeof(Patch_EscapingPrisoner).GetMethod(nameof(IsReadyToEscape)));
            yield return new CodeInstruction(OpCodes.Brtrue, restOfMethod);
            yield return new CodeInstruction(OpCodes.Ldnull);
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach (var ci in instr)
            {
                if (first)
                {
                    ci.labels.Add(restOfMethod);
                    first = false;
                }
                yield return ci;
            }
        }

        public static bool IsReadyToEscape(Pawn pawn)
        {
            Need_Motivation need = pawn.needs.TryGetNeed<Need_Motivation>();
            if (need != null && !need.ReadyToEscape)
                return false;
            else
                return true;
        }
    }
}
