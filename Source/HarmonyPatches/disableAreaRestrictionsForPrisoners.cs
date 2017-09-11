using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor.Harmony
{
    [HarmonyPatch(typeof(PawnColumnWorker_AllowedArea))]
    [HarmonyPatch("DoCell")]
    [HarmonyPatch(new[] {typeof(Rect), typeof(Pawn), typeof(PawnTable)})]
    //(Rect rect, Pawn pawn, PawnTable table)
    internal class disableAreaRestrictionsForPrisoners
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            var jumpTo = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Call,
                typeof(disableAreaRestrictionsForPrisoners).GetMethod("isPrisoner"));
            yield return new CodeInstruction(OpCodes.Brfalse, jumpTo);
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

        public static bool isPrisoner(Pawn pawn)
        {
            if (pawn.IsPrisoner)
                return true;
            return false;
        }
    }
}