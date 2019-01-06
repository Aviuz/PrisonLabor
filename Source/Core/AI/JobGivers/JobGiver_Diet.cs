using PrisonLabor.Core.Needs;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobGivers
{
    internal class JobGiver_Diet : JobGiver_GetFood
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
            if (pawn.needs.food.CurCategory < HungerCategory.Starving && FoodUtility.ShouldBeFedBySomeone(pawn))
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
                need.IsPrisonerWorking = false;

            return base.TryGiveJob(pawn);
        }
    }
}