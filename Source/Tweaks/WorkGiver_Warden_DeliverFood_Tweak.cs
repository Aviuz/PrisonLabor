using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    class WorkGiver_Warden_DeliverFood_Tweak : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!base.ShouldTakeCareOfPrisoner(pawn, t))
            {
                Messages.Message("one", MessageSound.Negative);
                return null;
            }
            Pawn pawn2 = (Pawn)t;
            if (!pawn2.guest.CanBeBroughtFood)
            {
                Messages.Message("two", MessageSound.Negative);
                return null;
            }
            //TODO test this condition
            IntVec3 c;
            //if (pawn2.Position.IsInPrisonCell(pawn2.Map) || RCellFinder.TryFindBestExitSpot((Pawn)t, out c, TraverseMode.ByPawn))
            if(false)
            {
                return null;
            }
            if (pawn2.needs.food.CurLevelPercentage >= pawn2.needs.food.PercentageThreshHungry + 0.02f)
            {
                Messages.Message("three", MessageSound.Negative);
                return null;
            }
            if (WardenFeedUtility.ShouldBeFed(pawn2))
            {
                Messages.Message("four", MessageSound.Negative);
                return null;
            }
            Thing thing;
            ThingDef def;
            if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn2, pawn2.needs.food.CurCategory == HungerCategory.Starving, out thing, out def, false, true, false, false, false))
            {
                Messages.Message("five", MessageSound.Negative);
                return null;
            }
            if (thing.GetRoom(RegionType.Set_Passable) == pawn2.GetRoom(RegionType.Set_Passable))
            {
                Messages.Message("six", MessageSound.Negative);
                return null;
            }
            if (WorkGiver_Warden_DeliverFood_Tweak.FoodAvailableInRoomTo(pawn2))
            {
                Messages.Message("seven", MessageSound.Negative);
                return null;
            }
            return new Job(JobDefOf.DeliverFood, thing, pawn2)
            {
                count = FoodUtility.WillIngestStackCountOf(pawn2, def),
                targetC = RCellFinder.SpotToChewStandingNear(pawn2, thing)
            };
        }

        private static bool FoodAvailableInRoomTo(Pawn prisoner)
        {
            if (prisoner.carryTracker.CarriedThing != null && WorkGiver_Warden_DeliverFood_Tweak.NutritionAvailableForFrom(prisoner, prisoner.carryTracker.CarriedThing) > 0f)
            {
                return true;
            }
            float num = 0f;
            float num2 = 0f;
            Room room = prisoner.GetRoom(RegionType.Set_Passable);
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
                    if (!thing.def.IsIngestible || thing.def.ingestible.preferability > FoodPreferability.DesperateOnly)
                    {
                        num2 += WorkGiver_Warden_DeliverFood_Tweak.NutritionAvailableForFrom(prisoner, thing);
                    }
                }
                List<Thing> list2 = region.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
                for (int k = 0; k < list2.Count; k++)
                {
                    Pawn pawn = list2[k] as Pawn;
                    if (pawn.IsPrisonerOfColony && pawn.needs.food.CurLevelPercentage < pawn.needs.food.PercentageThreshHungry + 0.02f && (pawn.carryTracker.CarriedThing == null || !pawn.RaceProps.WillAutomaticallyEat(pawn.carryTracker.CarriedThing)))
                    {
                        num += pawn.needs.food.NutritionWanted;
                    }
                }
            }
            return num2 + 0.5f >= num;
        }

        private static float NutritionAvailableForFrom(Pawn p, Thing foodSource)
        {
            if (foodSource.def.IsNutritionGivingIngestible && p.RaceProps.WillAutomaticallyEat(foodSource))
            {
                return foodSource.def.ingestible.nutrition * (float)foodSource.stackCount;
            }
            if (p.RaceProps.ToolUser && p.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                Building_NutrientPasteDispenser building_NutrientPasteDispenser = foodSource as Building_NutrientPasteDispenser;
                if (building_NutrientPasteDispenser != null && building_NutrientPasteDispenser.CanDispenseNow)
                {
                    return 99999f;
                }
            }
            return 0f;
        }
    }
}
