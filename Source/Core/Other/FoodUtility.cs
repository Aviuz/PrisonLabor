using System.Collections.Generic;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.Other
{
    public static class PrisonFoodUtility
    {
        // NOTE: from rimworld assembly
        // RimWorld.WorkGiver_Warden_DeliverFood
        public static bool FoodAvailableInRoomTo(Pawn prisoner)
        {
            if (prisoner.carryTracker.CarriedThing != null && NutritionAvailableForFrom(prisoner, prisoner.carryTracker.CarriedThing) > 0f)
            {
                return true;
            }
            float num = 0f;
            float num2 = 0f;
            Room room = prisoner.GetRoom();
            if (room == null)
            {
                return false;
            }
            for (int i = 0; i < room.RegionCount; i++)
            {
                Region region = room.Regions[i];
                List<Thing> list = region.ListerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
                for (int j = 0; j < list.Count; j++)
                {
                    Thing thing = list[j];
                    if (!thing.def.IsIngestible || (int)thing.def.ingestible.preferability > 3)
                    {
                        num2 += NutritionAvailableForFrom(prisoner, thing);
                    }
                }
                List<Thing> list2 = region.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
                for (int k = 0; k < list2.Count; k++)
                {
                    Pawn pawn = list2[k] as Pawn;
                    if (pawn.IsPrisonerOfColony && pawn.needs.food.CurLevelPercentage < pawn.needs.food.PercentageThreshHungry + 0.02f && (pawn.carryTracker.CarriedThing == null || !pawn.WillEat(pawn.carryTracker.CarriedThing)))
                    {
                        num += pawn.needs.food.NutritionWanted;
                    }
                }
            }
            if (num2 + 0.5f >= num)
            {
                return true;
            }
            return false;
        }

        public static float NutritionAvailableForFrom(Pawn p, Thing foodSource)
        {
            if (foodSource.def.IsNutritionGivingIngestible && p.WillEat(foodSource))
            {
                return foodSource.GetStatValue(StatDefOf.Nutrition) * (float)foodSource.stackCount;
            }
            if (p.RaceProps.ToolUser && p.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                Building_NutrientPasteDispenser building_NutrientPasteDispenser = foodSource as Building_NutrientPasteDispenser;
                if (building_NutrientPasteDispenser != null && building_NutrientPasteDispenser.CanDispenseNow && p.CanReach(building_NutrientPasteDispenser.InteractionCell, PathEndMode.OnCell, Danger.Some))
                {
                    return 99999f;
                }
            }

            return 0f;
        }
    }
}
