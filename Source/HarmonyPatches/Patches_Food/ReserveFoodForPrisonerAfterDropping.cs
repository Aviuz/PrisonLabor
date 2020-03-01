using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Food
{
    /// <summary>
    /// Adds food reservation after dropping food for prisoner
    /// </summary>
    [HarmonyPatch(typeof(JobDriver_FoodDeliver))]
    [HarmonyPatch("MakeNewToils")]
    static class ReserveFoodForPrisonerAfterDropping
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
}
