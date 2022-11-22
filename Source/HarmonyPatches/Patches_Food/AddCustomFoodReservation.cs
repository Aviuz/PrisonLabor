using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Food
{
    /// <summary>
    /// Adds check if food is already reserved before trying to bring it
    /// </summary>
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch(nameof(FoodUtility.BestFoodSourceOnMap_NewTemp))]
    static class AddCustomFoodReservation
    {
        /* === Orignal code Look-up===
         * 
         * <BestFoodSourceOnMap>c__AnonStorey.foodValidator = delegate(Thing t)
         * 
         * === CIL Instructions ===
         * 
         * ldloc.0 |  | Label 6Label 8
         * ldloc.0 |  | no labels
         * ldftn | Boolean <>m__0(Verse.Thing) | no labels
         * newobj | Void .ctor(Object, IntPtr) | no labels
         * stfld | System.Predicate`1[Verse.Thing] foodValidator | no labels
         * 
         */

        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            OpCode[] opCodes =
            {
                    OpCodes.Ldftn,
                    OpCodes.Newobj,
                    OpCodes.Stfld,
                };
            string[] operands =
            {
                // TODO this is a workaround
                    "", // "Boolean <>b__0(Verse.Thing)",
                    "Void .ctor(Object, IntPtr)",
                    "foodValidator",
                };
            int step = 0;

            foreach (var instr in instructions)
            {
                if (HPatcher.IsFragment(opCodes, operands, instr, ref step, nameof(AddCustomFoodReservation), false))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Call, typeof(AddCustomFoodReservation).GetMethod(nameof(AddContitionToPredicate)));
                }
                yield return instr;
            }
        }

        public static Predicate<Thing> AddContitionToPredicate(Predicate<Thing> predicate, Pawn getter, Pawn eater, bool desperate)
        {
            return new Predicate<Thing>(target =>
            {
                if (PrisonerFoodReservation.IsReserved(target) && (eater != getter || !eater.IsPrisoner) && !desperate)
                    return false;
                else
                    return predicate.Invoke(target);
            }
            );
        }
    }
}
