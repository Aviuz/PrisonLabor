using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
    [HarmonyPatch(typeof(RefuelWorkGiverUtility), "CanRefuel")]
    class Patch_WorkGiver_Refuel
    {
        static bool Postfix(bool __result, Pawn pawn, Thing t, bool forced)
        {
            if (!__result && pawn.IsPrisonerOfColony)
            {
				return CanRefuel(pawn, t, forced);
			}
            return __result;
        }


		private static bool CanRefuel(Pawn pawn, Thing t, bool forced)
        {
			CompRefuelable compRefuelable = t.TryGetComp<CompRefuelable>();
			if (compRefuelable == null || compRefuelable.IsFull || !compRefuelable.allowAutoRefuel || !compRefuelable.ShouldAutoRefuelNow)
			{
				return false;
			}
			if (t.IsForbiddenForPrisoner(pawn) || !pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1, -1, null, forced))
			{
				return false;
			}

			if (Traverse.Create(typeof(RefuelWorkGiverUtility)).Method("FindBestFuel", new[] { pawn, t }).GetValue<Thing>() == null)
			{
				ThingFilter fuelFilter = t.TryGetComp<CompRefuelable>().Props.fuelFilter;
				JobFailReason.Is("NoFuelToRefuel".Translate(fuelFilter.Summary));
				return false;
			}
			if (t.TryGetComp<CompRefuelable>().Props.atomicFueling && Traverse.Create(typeof(RefuelWorkGiverUtility)).Method("FindAllFuel", new[] { pawn, t }).GetValue<List<Thing>>() == null)
			{
				ThingFilter fuelFilter2 = t.TryGetComp<CompRefuelable>().Props.fuelFilter;
				JobFailReason.Is("NoFuelToRefuel".Translate(fuelFilter2.Summary));
				return false;
			}
			return true;
		}
    }
}
