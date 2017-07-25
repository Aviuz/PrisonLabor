using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    class JobGiver_Diet : ThinkNode_JobGiver
    {
        private HungerCategory minCategory = HungerCategory.Hungry;
        private HungerCategory stopWorkingCat = HungerCategory.UrgentlyHungry;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_Diet jobGiver_Diet = (JobGiver_Diet)base.DeepCopy(resolve);
            jobGiver_Diet.minCategory = this.minCategory;
            return jobGiver_Diet;
        }

        public override float GetPriority(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            if (food == null)
            {
                return 0f;
            }
            if (pawn.needs.food.CurCategory < HungerCategory.Starving && FoodUtility_Tweak.ShouldBeFedBySomeone(pawn))
            {
                return 0f;
            }
            if (food.CurCategory < this.minCategory)
            {
                return 0f;
            }
            if (food.CurCategory <= stopWorkingCat)
            {
                return 11f;
            }
            if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
            {
                return 7f;
            }
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            if (food == null || food.CurCategory < this.minCategory)
            {
                return null;
            }
            if(pawn.needs.TryGetNeed<Need_Motivation>() != null)
                pawn.needs.TryGetNeed<Need_Motivation>().Enabled = false;
            bool flag;
            if (pawn.RaceProps.Animal)
            {
                flag = true;
            }
            else
            {
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false);
                flag = (firstHediffOfDef != null && firstHediffOfDef.Severity > 0.4f);
            }
            bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;
            bool allowCorpse = flag;
            Thing thing;
            ThingDef def;
            if (!FoodUtility_Tweak.TryFindBestFoodSourceFor(pawn, pawn, desperate, out thing, out def, true, true, true, allowCorpse, false))
            {
                return null;
            }
            Pawn pawn2 = thing as Pawn;
            if (pawn2 != null)
            {
                return new Job(JobDefOf.PredatorHunt, pawn2)
                {
                    killIncappedTarget = true
                };
            }
            Building_NutrientPasteDispenser building_NutrientPasteDispenser = thing as Building_NutrientPasteDispenser;
            if (building_NutrientPasteDispenser != null && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
            {
                Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
                if (building != null)
                {
                    ISlotGroupParent hopperSgp = building as ISlotGroupParent;
                    Job job = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
                    if (job != null)
                    {
                        return job;
                    }
                }
                thing = FoodUtility_Tweak.BestFoodSourceOnMap(pawn, pawn, desperate, FoodPreferability.MealLavish, false, !pawn.IsTeetotaler(), false, false, false, false, false);
                if (thing == null)
                {
                    return null;
                }
                def = thing.def;
            }
            return new Job(JobDefOf.Ingest, thing)
            {
                count = FoodUtility_Tweak.WillIngestStackCountOf(pawn, def)
            };
        }
    }
}
