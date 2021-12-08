using HarmonyLib;
using PrisonLabor.WorkUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Construction
{
    [HarmonyPatch(typeof(WorkGiver_ConstructDeliverResourcesToFrames))]
    [HarmonyPatch("JobOnThing")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(Thing), typeof(bool) })]
    class Patch_WorkGiver_ConstructDeliverResourcesToFrames
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

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
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, typeof(ConstructionUtils).GetMethod(nameof(ConstructionUtils.IsPrisonerWork)));
            yield return new CodeInstruction(OpCodes.Brtrue, label);

            foreach (var instr in instructions)
            {
                yield return instr;
            }            
         
        }
    }
}
