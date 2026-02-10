using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch(typeof(WorkGiver_FeedPatient), "HasJobOnThing")]
  public class Patch_WorkGiver_DeliverFood
  {
    static bool Postfix(bool __result, Pawn pawn, Thing t, bool forced)
    {
      if (__result)
      {
        return true;
      }

      if (!pawn.IsPrisonerOfColony || !(t is Pawn inmate))
      {
        return false;
      }

      if (!ShouldTakeCareOfPrisoner(pawn, t))
      {
        return false;
      }

      if (pawn.DevelopmentalStage.Baby() || inmate.DevelopmentalStage.Baby())
      {
        return false;
      }

      if (!WardenFeedUtility.ShouldBeFed(inmate))
      {
        return false;
      }

      if (inmate.needs.food == null)
      {
        return false;
      }

      if (inmate.needs.food.CurLevelPercentage >= inmate.needs.food.PercentageThreshHungry + 0.02f)
      {
        return false;
      }

      var currentRespectedRestriction = inmate.foodRestriction?.GetCurrentRespectedRestriction(pawn);
      if (currentRespectedRestriction != null && currentRespectedRestriction.filter.AllowedDefCount == 0)
      {
        JobFailReason.Is("NoFoodMatchingRestrictions".Translate());
        return false;
      }

      if (FoodUtility.TryFindBestFoodSourceFor(pawn, inmate, inmate.needs.food.CurCategory == HungerCategory.Starving,
            out _, out _, canRefillDispenser: false, canUseInventory: true,
            canUsePackAnimalInventory: false, allowForbidden: false, allowCorpse: false))
      {
        return true;
      }

      JobFailReason.Is("NoFood".Translate());
      return false;
    }


    private static bool ShouldTakeCareOfPrisoner(Pawn warden, Thing prisoner, bool forced = false)
    {
      return prisoner is Pawn pawn && pawn.IsPrisonerOfColony && pawn.guest.PrisonerIsSecure && pawn.Spawned &&
             !pawn.InAggroMentalState && !prisoner.IsForbidden(warden) && !pawn.IsFormingCaravan() &&
             warden.CanReserveAndReach(pawn, PathEndMode.OnCell, warden.NormalMaxDanger(),
               ignoreOtherReservations: forced);
    }
  }
}