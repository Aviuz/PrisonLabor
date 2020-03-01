using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Food
{
    /// <summary>
    /// Complete overhaul PawnCanAutomaticallyHaulFast check, to include FoodReservation
    /// </summary>
    [HarmonyPatch(typeof(HaulAIUtility))]
    [HarmonyPatch("PawnCanAutomaticallyHaulFast")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(Thing), typeof(bool) })]
    static class ReservedByPrisonerPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            //Load arguments onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            //Call function
            yield return new CodeInstruction(OpCodes.Call,
                typeof(ReservedByPrisonerPatch).GetMethod(nameof(CanHaulAndInPrisonCell)));
            //Return
            yield return new CodeInstruction(OpCodes.Ret);
        }


        public static bool CanHaulAndInPrisonCell(Pawn p, Thing t, bool forced)
        {
            var unfinishedThing = t as UnfinishedThing;
            if (unfinishedThing != null && unfinishedThing.BoundBill != null)
                return false;
            if (!p.CanReach(t, PathEndMode.ClosestTouch, p.NormalMaxDanger(), false, TraverseMode.ByPawn))
                return false;
            if (!p.CanReserve(t, 1, -1, null, forced))
                return false;
            if (t.def.IsNutritionGivingIngestible && t.def.ingestible.HumanEdible &&
                !t.IsSociallyProper(p, false, true))
                if (PrisonerFoodReservation.IsReserved(t))
                {
                    JobFailReason.Is("ReservedForPrisoners".Translate());
                    return false;
                }
            if (t.IsBurning())
            {
                JobFailReason.Is("BurningLower".Translate());
                return false;
            }
            return true;
        }
    }
}