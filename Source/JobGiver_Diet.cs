using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    internal class JobGiver_Diet : ThinkNode_JobGiver
    {
        private HungerCategory minCategory = HungerCategory.Hungry;
        private readonly HungerCategory stopWorkingCat = HungerCategory.UrgentlyHungry;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var jobGiver_Diet = (JobGiver_Diet)base.DeepCopy(resolve);
            jobGiver_Diet.minCategory = minCategory;
            return jobGiver_Diet;
        }

        public override float GetPriority(Pawn pawn)
        {
            var food = pawn.needs.food;
            if (food == null)
                return 0f;
            if (pawn.needs.food.CurCategory < HungerCategory.Starving && FoodUtility_Tweak.ShouldBeFedBySomeone(pawn))
                return 0f;
            if (food.CurCategory < minCategory)
                return 0f;
            if (food.CurCategory <= stopWorkingCat)
                return 11f;
            if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
                return 7f;
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            var food = pawn.needs.food;
            if (food == null || food.CurCategory < minCategory)
                return null;
            var need = pawn.needs.TryGetNeed<Need_Motivation>();
            if (need != null)
                need.PrisonerWorking = false;
            bool flag;
            if (pawn.RaceProps.Animal)
            {
                flag = true;
            }
            else
            {
                var firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false);
                flag = firstHediffOfDef != null && firstHediffOfDef.Severity > 0.4f;
            }
            var desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;
            var allowCorpse = flag;
            Thing thing;
            ThingDef def;
            if (!FoodUtility_Tweak.TryFindBestFoodSourceFor(pawn, pawn, desperate, out thing, out def, true, true, true,
                allowCorpse, false))
                return null;
            var pawn2 = thing as Pawn;
            if (pawn2 != null)
                return new Job(JobDefOf.PredatorHunt, pawn2)
                {
                    killIncappedTarget = true
                };
            var building_NutrientPasteDispenser = thing as Building_NutrientPasteDispenser;
            if (building_NutrientPasteDispenser != null &&
                !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
            {
                var building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
                if (building != null)
                {
                    var hopperSgp = building as ISlotGroupParent;
                    var job = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
                    if (job != null)
                        return job;
                }
                thing = FoodUtility_Tweak.BestFoodSourceOnMap(pawn, pawn, desperate, FoodPreferability.MealLavish,
                    false, !pawn.IsTeetotaler(), false, false, false, false, false);
                if (thing == null)
                    return null;
                def = thing.def;
            }
            return new Job(JobDefOf.Ingest, thing)
            {
                count = FoodUtility_Tweak.WillIngestStackCountOf(pawn, def)
            };
        }
    }
}