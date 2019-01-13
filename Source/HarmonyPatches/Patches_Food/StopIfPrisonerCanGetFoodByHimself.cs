using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Food
{
    /// <summary>
    /// This patch is preventing wardens to deliver food when prisoner can get it by himself.
    /// There is already some kind of this mechanic in Vanilla RimWorld, but it only affect room that prisoner is inside.
    /// </summary>
    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood))]
    [HarmonyPatch(nameof(WorkGiver_Warden_DeliverFood.JobOnThing))]
    static class StopIfPrisonerCanGetFoodByHimself
    {
        /*  === Orignal code Look-up===
         * 
         *  if (WorkGiver_Warden_DeliverFood.FoodAvailableInRoomTo(pawn2))
		 *	{
		 *		return null;
		 *	}
         *  
         *  === CIL Instructions ===
         *  
         *  ldloc.0 |  | Label 7
         *  call | Boolean FoodAvailableInRoomTo(Verse.Pawn) | no labels
         *  brfalse | Label 8 | no labels
         *  ldnull |  | no labels
         *  ret |  | no labels
         *  ldloc.2 |  | Label 8
         */

        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            OpCode[] opCodes =
            {
                OpCodes.Call,
            };
            string[] operands =
            {
                "Boolean FoodAvailableInRoomTo(Verse.Pawn)",
            };
            int step = 0;

            foreach (var instr in instructions)
            {
                if (HPatcher.IsFragment(opCodes, operands, instr, ref step, nameof(StopIfPrisonerCanGetFoodByHimself) + 1, true))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(StopIfPrisonerCanGetFoodByHimself).GetMethod(nameof(FoodAvailableForPrisoner)));
                }
                else
                {
                    yield return instr;
                }
            }
        }

        public static bool FoodAvailableForPrisoner(Pawn pawn)
        {
            Thing thing;
            ThingDef thingDef;

            return FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, false, out thing, out thingDef, true, true, false, false, true, pawn.IsWildMan());
        }
    }
}
