using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Construction
{
    [HarmonyPatch(typeof(WorkGiver_ConstructFinishFrames))]
    [HarmonyPatch(nameof(WorkGiver_ConstructFinishFrames.JobOnThing))]
    static class EnableConstructionFinishFrames
    {
        /*  === Orignal code Look-up===
         *  
         *  if (t.Faction != pawn.Faction)
		 *	{
		 *		return false;
		 *	}
         *  
         *  === CIL Instructions ===
         *  
         *  ldarg.2 |  | no labels
         *  callvirt | RimWorld.Faction get_Faction() | no labels
         *  ldarg.1 |  | no labels
         *  callvirt | RimWorld.Faction get_Faction() | no labels
         *  beq | Label 1 | no labels
         *  ldc.i4.0 |  | no labels
         *  ret |  | no labels
         *  ldarg.2 |  | Label 1
         */

        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
        {
            //find label to jump
            OpCode[] opCodes1 =
            {
                OpCodes.Ldarg_2,
                OpCodes.Callvirt,
                OpCodes.Ldarg_1,
                OpCodes.Callvirt,
                OpCodes.Beq_S,
            };
            string[] operands1 =
            {
                "",
                "RimWorld.Faction get_Faction()",
                "",
                "RimWorld.Faction get_Faction()",
                "System.Reflection.Emit.Label",
            };
            var label = HPatcher.FindOperandAfter(opCodes1, operands1, instructions, true);

            //Add If(pawn.IsPrisonerOfColony) {jump next condition}
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Callvirt, typeof(Pawn).GetProperty(nameof(Pawn.IsPrisoner)).GetGetMethod());
            yield return new CodeInstruction(OpCodes.Brtrue, label);

            foreach (var instr in instructions)
            {
                yield return instr;
            }
        }
    }
}
