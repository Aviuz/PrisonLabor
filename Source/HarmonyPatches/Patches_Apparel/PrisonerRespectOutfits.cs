using HarmonyLib;
using PrisonLabor.Core.BillAssignation;
using PrisonLabor.Core.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Apparel
{
  [HarmonyPatch(typeof(JobGiver_PrisonerGetDressed))]
  public class PrisonerRespectOutfits
  {
    [HarmonyPostfix]
    [HarmonyPatch("FindGarmentCoveringPart")]
    static Apparel Postfix_FindGarmentCoveringPart(Apparel __result, Pawn pawn, BodyPartGroupDef bodyPartGroupDef)
    {
      return PrisonerWillRespectOutfit(__result, pawn);
    }

    [HarmonyPostfix]
    [HarmonyPatch("FindGarmentSatisfyingTitleRequirement")]
    static Apparel Postfix_FindGarmentSatisfyingTitleRequirement(Apparel __result, Pawn pawn, ApparelRequirement req)
    {
      return PrisonerWillRespectOutfit(__result, pawn);
    }

    private static Apparel PrisonerWillRespectOutfit(Apparel apparel, Pawn prisoner)
    {
      if (apparel != null && IsMotivatedPrisoner(prisoner) && prisoner.outfits != null && !prisoner.outfits.CurrentOutfit.filter.Allows(apparel.def))
      {
        return null;
      }
      return apparel;

    }

    private static bool IsMotivatedPrisoner(Pawn pawn)
    {
      return pawn.IsPrisonerOfColony && pawn.IsMotivated();
    }
  }
}
