using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    /// <summary>
    ///     Add checking if food is reserved by prisoner
    /// </summary>
    [HarmonyPatch(typeof(ForbidUtility))]
    [HarmonyPatch("IsForbidden")]
    [HarmonyPatch(new[] {typeof(Thing), typeof(Pawn)})]
    internal class Patch_ItemIsForbidden
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            var endOfPatch = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, typeof(PrisonerFoodReservation).GetMethod("isReserved"));
            yield return new CodeInstruction(OpCodes.Brfalse, endOfPatch);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, typeof(Pawn).GetMethod("get_IsPrisoner"));
            yield return new CodeInstruction(OpCodes.Brtrue, endOfPatch);
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Ret);

            var first = true;
            foreach (var ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(endOfPatch);
                }
                yield return ci;
            }
        }
    }
}