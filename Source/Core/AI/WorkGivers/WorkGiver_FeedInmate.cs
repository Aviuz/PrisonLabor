using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.WorkGivers
{
  public class WorkGiver_FeedInmate : WorkGiver_Warden_Feed
  {
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
      return pawn.Map.mapPawns.PrisonersOfColonySpawned.Where(FeedPatientUtility.IsHungry);
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
      return !PotentialWorkThingsGlobal(pawn).Any();
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
      if (!(t is Pawn inmate))
      {
        return null;
      }

      if (!pawn.IsPrisonerOfColony)
      {
        return null;
      }

      if (!ShouldTakeCareOfPrisoner(pawn, t))
      {
        return null;
      }

      if (pawn.DevelopmentalStage.Baby() || inmate.DevelopmentalStage.Baby())
      {
        return null;
      }

      if (!WardenFeedUtility.ShouldBeFed(inmate))
      {
        return null;
      }

      if (inmate.needs.food == null)
      {
        return null;
      }

      if (inmate.needs.food.CurLevelPercentage >= inmate.needs.food.PercentageThreshHungry + 0.02f)
      {
        return null;
      }

      var currentRespectedRestriction = inmate.foodRestriction?.GetCurrentRespectedRestriction(pawn);
      if (currentRespectedRestriction != null && currentRespectedRestriction.filter.AllowedDefCount == 0)
      {
        JobFailReason.Is("NoFoodMatchingRestrictions".Translate());
        return null;
      }
      if (!FoodUtility.TryFindBestFoodSourceFor(pawn, inmate, inmate.needs.food.CurCategory == HungerCategory.Starving, out var foodSource, out var foodDef, canRefillDispenser: false, canUseInventory: true, canUsePackAnimalInventory: false, allowForbidden: false, allowCorpse: false))
      {
        JobFailReason.Is("NoFood".Translate());
        return null;
      }
      var nutrition = FoodUtility.GetNutrition(inmate, foodSource, foodDef);
      var job = JobMaker.MakeJob(JobDefOf.FeedPatient, foodSource, inmate);
      job.count = FoodUtility.WillIngestStackCountOf(inmate, foodDef, nutrition);
      return job;
    }
  }
}