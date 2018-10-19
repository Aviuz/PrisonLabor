using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    internal class WorkGiver_Warden_DeliverFood_Tweak : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!ShouldTakeCareOfPrisoner(pawn, t))
                return null;
            var pawn2 = (Pawn) t;
            if (!pawn2.guest.CanBeBroughtFood)
                return null;
            //if (pawn2.Position.IsInPrisonCell(pawn2.Map) || RCellFinder.TryFindBestExitSpot((Pawn)t, out c, TraverseMode.ByPawn))
            if (pawn2.needs.food.CurLevelPercentage >= pawn2.needs.food.PercentageThreshHungry + 0.02f)
                return null;
            if (WardenFeedUtility.ShouldBeFed(pawn2))
                return null;
            Thing thing;
            ThingDef def;
            //Tweak: changes way of finding food
            if (!FoodUtility_Tweak.TryFindBestFoodSourceFor(pawn, pawn2,
                pawn2.needs.food.CurCategory == HungerCategory.Starving, out thing, out def, false, true, false, false,
                false))
                return null;
            if (thing.GetRoom(RegionType.Set_Passable) == pawn2.GetRoom(RegionType.Set_Passable))
                return null;
            if (FoodAvailableInRoomTo(pawn2))
                return null;
            return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_DeliverFood_Tweak"), thing, pawn2)
            {
                count = FoodUtility_Tweak.WillIngestStackCountOf(pawn2, def),
                targetC = RCellFinder.SpotToChewStandingNear(pawn2, thing)
            };
        }

        private static bool FoodAvailableInRoomTo(Pawn prisoner)
        {
            if (prisoner.carryTracker.CarriedThing != null &&
                NutritionAvailableForFrom(prisoner, prisoner.carryTracker.CarriedThing) > 0f)
                return true;
            var num = 0f;
            var num2 = 0f;
            var room = prisoner.GetRoom(RegionType.Set_Passable);
            if (room == null)
                return false;
            for (var i = 0; i < room.RegionCount; i++)
            {
                var region = room.Regions[i];
                var list = region.ListerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
                for (var j = 0; j < list.Count; j++)
                {
                    var thing = list[j];
                    if (!thing.def.IsIngestible || thing.def.ingestible.preferability > FoodPreferability.DesperateOnly)
                        num2 += NutritionAvailableForFrom(prisoner, thing);
                }
                var list2 = region.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
                for (var k = 0; k < list2.Count; k++)
                {
                    var pawn = list2[k] as Pawn;
                    if (pawn.IsPrisonerOfColony &&
                        pawn.needs.food.CurLevelPercentage < pawn.needs.food.PercentageThreshHungry + 0.02f &&
                        (pawn.carryTracker.CarriedThing == null ||
                         !pawn.WillEat(pawn.carryTracker.CarriedThing)))
                        num += pawn.needs.food.NutritionWanted;
                }
            }
            return num2 + 0.5f >= num;
        }

        private static float NutritionAvailableForFrom(Pawn p, Thing foodSource)
        {
            if (foodSource.def.IsNutritionGivingIngestible && p.WillEat(foodSource))
                return foodSource.def.ingestible.CachedNutrition * foodSource.stackCount;
            if (p.RaceProps.ToolUser && p.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                var building_NutrientPasteDispenser = foodSource as Building_NutrientPasteDispenser;
                if (building_NutrientPasteDispenser != null && building_NutrientPasteDispenser.CanDispenseNow)
                    return 99999f;
            }
            return 0f;
        }
    }
}