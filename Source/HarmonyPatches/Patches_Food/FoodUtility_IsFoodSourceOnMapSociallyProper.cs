using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PrisonLabor.HarmonyPatches.Patches_Food
{
    /// <summary>
    /// This patch is ignoring socially improper
    /// </summary>
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch("IsFoodSourceOnMapSociallyProper")]
    static class FoodUtility_IsFoodSourceOnMapSociallyProper
    {
        /*  === Original code Look-up ===
         *  
         *  if (!allowSociallyImproper)
         *  {
         *  	bool animalsCare = !getter.RaceProps.Animal;
         *  	if (!t.IsSociallyProper(getter) && !t.IsSociallyProper(eater, eater.IsPrisonerOfColony, animalsCare))
         *  	{
         *  		return false;
         *  	}
         *  }
         *  return true;
         *  
         *  === CIL Instructions ===
         *  
         *  ldarg.3 |  | no labels
         *  brtrue | Label 1 | no labels
         *  ldarg.1 |  | no labels
         *  callvirt | Verse.RaceProperties get_RaceProps() | no labels
         *  callvirt | Boolean get_Animal() | no labels
         *  ldc.i4.0 |  | no labels
         *  ceq |  | no labels
         *  stloc.0 |  | no labels
         *  ldarg.0 |  | no labels
         *  ldarg.1 |  | no labels
         *  call | Boolean IsSociallyProper(Verse.Thing, Verse.Pawn) | no labels
         *  brtrue | Label 2 | no labels
         *  ldarg.0 |  | no labels
         *  ldarg.2 |  | no labels
         *  ldarg.2 |  | no labels
         *  callvirt | Boolean get_IsPrisonerOfColony() | no labels
         *  ldloc.0 |  | no labels
         *  call | Boolean IsSociallyProper(Verse.Thing, Verse.Pawn, Boolean, Boolean) | no labels
         *  brtrue | Label 3 | no labels
         *  ldc.i4.0 |  | no labels
         *  ret |  | no labels
         *  ldc.i4.1 |  | Label 1Label 2Label 3
         *  ret |  | no labels
         */

        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            OpCode[] opCodes =
            {
                    OpCodes.Ldarg_3,
                    OpCodes.Brtrue,
                };
            string[] operands =
            {
                    "",
                    "",
                };
            int step = 0;

            foreach (var instr in instructions)
            {
                if (HPatcher.IsFragment(opCodes, operands, instr, ref step, nameof(Patch_FoodDeliver) + nameof(FoodUtility_IsFoodSourceOnMapSociallyProper), false))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    instr.opcode = OpCodes.Br;
                }
                yield return instr;
            }
        }
    }
}
