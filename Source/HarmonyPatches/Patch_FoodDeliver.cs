using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches
{
    static class Patch_FoodDeliver
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

        /// <summary>
        /// Adds check if food is already reserved before trying to bring it
        /// </summary>
        [HarmonyPatch(typeof(FoodUtility))]
        [HarmonyPatch(nameof(FoodUtility.BestFoodSourceOnMap))]
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
                    "Boolean <>m__0(Verse.Thing)",
                    "Void .ctor(Object, IntPtr)",
                    "foodValidator",
                };
                int step = 0;

                foreach (var instr in instructions)
                {
                    if (HPatcher.IsFragment(opCodes, operands, instr, ref step, nameof(Patch_FoodDeliver) + nameof(AddCustomFoodReservation), false))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        yield return new CodeInstruction(OpCodes.Call, typeof(AddCustomFoodReservation).GetMethod(nameof(AddContitionToPredicate)));
                    }
                    yield return instr;
                }
            }

            static Predicate<Thing> AddContitionToPredicate(Predicate<Thing> predicate, Pawn getter, Pawn eater, bool desperate)
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


        /// <summary>
        /// Adds food reservation after dropping food for prisoner
        /// </summary>
        [HarmonyPatch(typeof(JobDriver_FoodDeliver))]
        [HarmonyPatch("MakeNewToils")]
        static class Patch_ReserveFoodForPrisonerAfterDropping
        {
            static void Postfix(ref IEnumerable<Toil> __result, JobDriver_FoodDeliver __instance)
            {
                __result = new List<Toil>(__result);
                var lastToil = ((List<Toil>)__result).Last();

                lastToil.initAction = delegate
                {
                    Thing thing;
                    __instance.pawn.carryTracker.TryDropCarriedThing(lastToil.actor.jobs.curJob.targetC.Cell, ThingPlaceMode.Direct,
                        out thing, null);
                    PrisonerFoodReservation.reserve(thing, (Pawn)lastToil.actor.jobs.curJob.targetB.Thing);
                };
            }
        }

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
}
