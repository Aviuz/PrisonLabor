using HarmonyLib;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobGivers
{
  public class JobGiver_Prisoner_OptimizeApparel : JobGiver_OptimizeApparel
  {
    private static StringBuilder debugSb;
    private static List<float> wornApparelScores = new List<float>();

    protected override Job TryGiveJob(Pawn pawn)
    {
      if (!pawn.IsPrisonerOfColony)
      {
        return null;
      }
      if (pawn.IsQuestLodger())
      {
        return null;
      }
      if (pawn.outfits == null)
      {
        pawn.outfits = new Pawn_OutfitTracker(pawn);
      }

      if (!DebugViewSettings.debugApparelOptimize)
      {
        if (Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick)
        {
          return null;
        }
      }
      else
      {
        debugSb = new StringBuilder();
        debugSb.AppendLine($"Scanning for {pawn} at {pawn.Position}");
      }

      ApparelPolicy currentOutfit = pawn.outfits.CurrentApparelPolicy;
      List<Apparel> wornApparel = pawn.apparel.WornApparel;

      if (pawn.IsMotivated())
      {
        for (int num = wornApparel.Count - 1; num >= 0; num--)
        {
          if (!currentOutfit.filter.Allows(wornApparel[num]) && pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[num]) && !pawn.apparel.IsLocked(wornApparel[num]))
          {
            Job job2 = JobMaker.MakeJob(JobDefOf.RemoveApparel, wornApparel[num]);
            job2.haulDroppedApparel = true;
            DebugLogger.debug($"Prisoner {pawn.LabelShort} is removing: {wornApparel[num].def.defName} - called return");
            return job2;
          }
        }
      }
      else
      {
        DebugLogger.debug($"Prisoner {pawn.NameShortColored} not motivated. Removing apparel skiped");
      }

      Thing thing = null;
      float num2 = 0f;
      List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Apparel);
      if (list.Count == 0)
      {
        SetNextOptimizeTick(pawn);
        DebugLogger.debug($"No apparel found for prisoner {pawn.LabelShort}. Null return.");
        return null;
      }
      CalculateNeedWarmth(pawn);
      wornApparelScores.Clear();
      for (int i = 0; i < wornApparel.Count; i++)
      {
        wornApparelScores.Add(ApparelScoreRaw(pawn, wornApparel[i]));
      }
      for (int j = 0; j < list.Count; j++)
      {
        Apparel apparel = (Apparel)list[j];
        bool filterAllows = currentOutfit.filter.Allows(apparel);
        bool isStored = IsStored(apparel);
        bool notForbidden = !apparel.IsForbidden(pawn);
        bool notBurning = !apparel.IsBurning();
        bool genderMatching = IsGenderMatchingApparel(pawn, apparel);
        DebugLogger.debug($"Checking {apparel.def.defName} for {pawn.LabelShort}. Fillter allows: {filterAllows}, isStored: {isStored}," +
          $" notForbidden: {notForbidden}, notBurning: {notBurning}, gender Matching: {genderMatching}");
        if (filterAllows && isStored && notForbidden && notBurning && genderMatching)
        {
          float num3 = ApparelScoreGain(pawn, apparel, wornApparelScores);
          if (DebugViewSettings.debugApparelOptimize)
          {
            debugSb.AppendLine(apparel.LabelCap + ": " + num3.ToString("F2"));
          }
          if (!(num3 < 0.05f) && !(num3 < num2) && (!CompBiocodable.IsBiocoded(apparel) || CompBiocodable.IsBiocodedFor(apparel, pawn)) && ApparelUtility.HasPartsToWear(pawn, apparel.def) && pawn.CanReserveAndReach(apparel, PathEndMode.OnCell, pawn.NormalMaxDanger()) && apparel.def.apparel.developmentalStageFilter.Has(pawn.DevelopmentalStage))
          {
            thing = apparel;
            num2 = num3;
          }
        }
      }
      if (DebugViewSettings.debugApparelOptimize)
      {
        debugSb.AppendLine("BEST: " + thing);
        Log.Message(debugSb.ToString());
        debugSb = null;
      }
      if (thing == null)
      {
        SetNextOptimizeTick(pawn);
        DebugLogger.debug($"No matching apparel found for prisoner {pawn.LabelShort}. Null return.");
        return null;
      }
      return JobMaker.MakeJob(JobDefOf.Wear, thing);
    }

    private static bool IsGenderMatchingApparel(Pawn pawn, Apparel apparel)
    {
      return apparel.def.apparel.gender == Gender.None || apparel.def.apparel.gender == pawn.gender;
    }

    private static bool IsStored(Apparel apparel)
    {
      return apparel.GetRoom().IsPrisonCell || apparel.IsInAnyStorage();
    }

    private void CalculateNeedWarmth(Pawn pawn)
    {
      Traverse.Create(this).Field("neededWarmth").SetValue(PawnApparelGenerator.CalculateNeededWarmth(pawn, pawn.Map.Tile, GenLocalDate.Twelfth(pawn)));
    }

    private void SetNextOptimizeTick(Pawn pawn)
    {
      pawn.mindState.nextApparelOptimizeTick = Find.TickManager.TicksGame + Rand.Range(6000, 9000);
    }
  }
}
