using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Harmony
{
    [HarmonyPatch(typeof(HaulAIUtility))]
    [HarmonyPatch("PawnCanAutomaticallyHaulBasicChecks")]
    [HarmonyPatch(new[] {typeof(Pawn), typeof(Thing), typeof(bool)})]
    internal class ReservedByPrisonerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            //Load arguments onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            //Call function
            yield return new CodeInstruction(OpCodes.Call,
                typeof(ReservedByPrisonerPatch).GetMethod("CanHaulAndInPrisonCell"));
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
                if (PrisonerFoodReservation.isReserved(t))
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