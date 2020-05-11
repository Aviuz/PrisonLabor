using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_DeepDrill
{
    [HarmonyPatch(typeof(WorkGiver_DeepDrill))]
    static class EnableDeepDrillsToPrisoners
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

        /*        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
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
                }*/

        [HarmonyPostfix]
        [HarmonyPatch(nameof(WorkGiver_DeepDrill.HasJobOnThing))]
        [HarmonyPatch(new[] { typeof(Pawn), typeof(Thing), typeof(bool) })]
        static bool HasJobOnThingPostfix(bool __result, WorkGiver_DeepDrill __instance, Pawn pawn, Thing t, bool forced)
        {
            Building building = t as Building;
            if (building != null && pawn != null && building.Faction.IsPlayer)
            {
                if (pawn.IsPrisonerOfColony)
                {
                    return true;
                } else if (pawn.Faction.IsPlayer)
                {
                    return !PrisonLaborUtility.IsDisabledByLabor(building.Position, pawn, __instance.def.workType);
                }                
            }
            return __result;
        }

/*        [HarmonyPostfix]
        [HarmonyPatch(nameof(WorkGiver_DeepDrill.JobOnThing))]
        [HarmonyPatch(new[] { typeof(Pawn), typeof(Thing), typeof(bool) })]
        static Job JobOnThingPostfix(Job __result, WorkGiver_DeepDrill __instance, Pawn pawn, Thing t, bool forced)
        {
            Building building = t as Building;
            if (building != null && pawn != null)
            {
                if(PrisonLaborUtility.IsDisabledByLabor(building.Position, pawn, __instance.def.workType ) && !pawn.IsPrisonerOfColony)
                {
                    return null;
                }                
            }
            return __result;
        }*/
    }
}
