using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PrisonLabor.HarmonyPatches.Patches_Food
{
    /// <summary>
    /// This patch is ensuring prisoner will be brought food despite beign outside of prison cell.
    /// It skips the condition "IsInPrisonCell"
    /// </summary>
    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood))]
    [HarmonyPatch(nameof(WorkGiver_Warden_DeliverFood.JobOnThing))]
    static class DeliverEvenOutsidePrisonCell
    {
        /*  === Orignal code Look-up===
         * 
         *  if (!pawn2.Position.IsInPrisonCell(pawn2.Map))
         *  {
         *      return null;
         *  }
         *  
         *  === CIL Instructions ===
         *  
         *  ldloc.0 |  | Label 2
         *  callvirt | IntVec3 get_Position() | no labels
         *  ldloc.0 |  | no labels
         *  callvirt | Verse.Map get_Map() | no labels
         *  call | Boolean IsInPrisonCell(IntVec3, Verse.Map) | no labels
         *  brtrue | Label 3 | no labels
         *  ldnull |  | no labels
         *  ret |  | no labels
         *
         *  ldloc.0 |  | Label 3
         */

        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            OpCode[] opCodes =
            {
                    OpCodes.Call,
                    OpCodes.Brtrue,
                };
            string[] operands =
            {
                    "Boolean IsInPrisonCell(IntVec3, Verse.Map)",
                    "System.Reflection.Emit.Label",
                };
            int step = 0;

            foreach (var instr in instructions)
            {
                if (HPatcher.IsFragment(opCodes, operands, instr, ref step, nameof(Patch_FoodDeliver) + nameof(DeliverEvenOutsidePrisonCell), true))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    instr.opcode = OpCodes.Br;
                }
                yield return instr;
            }
        }
    }
}
