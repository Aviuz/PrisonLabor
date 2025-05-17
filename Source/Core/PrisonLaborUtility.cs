using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PrisonLabor.Constants;
using PrisonLabor.Core.LaborArea;
using PrisonLabor.Core.LaborWorkSettings;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Other;
using RimWorld;
using Verse;

namespace PrisonLabor.Core
{
  public static class PrisonLaborUtility
  {
    private static readonly Traverse IsSuitableMethod =
      Traverse.Create<ITab_Pawn_Visitor>().Method("IsStudiable", new[] { typeof(Pawn) });

    public static bool LaborEnabled(this Pawn pawn)
    {
      return pawn.IsPrisoner && pawn.guest.IsInteractionEnabled(PL_DefOf.PrisonLabor_workOption);
    }

    public static bool WorkTime(Pawn pawn)
    {
      if (pawn.timetable == null)
        return true;
      if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
        return true;
      if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything)
      {
        return !HealthAIUtility.ShouldSeekMedicalRest(pawn) &&
               !pawn.health.hediffSet.HasTemperatureInjury(TemperatureInjuryStage.Serious) &&
               !CheckFoodNeed(pawn) &&
               !CheckRestNeed(pawn);
      }

      return false;
    }

    private static bool CheckFoodNeed(Pawn pawn)
    {
      return pawn.needs?.food != null && pawn.needs.food.CurCategory > HungerCategory.Hungry;
    }

    private static bool CheckRestNeed(Pawn pawn)
    {
      return pawn.needs?.rest != null && pawn.needs.rest.CurCategory != RestCategory.Rested;
    }

    public static bool IsDisabledByLabor(IntVec3 pos, Pawn pawn, WorkTypeDef workType)
    {
      if (pos != null && pawn.Map.areaManager.Get<Area_Labor>() != null &&
          !WorkSettings.WorkDisabled(workType))
        return pawn.Map.areaManager.Get<Area_Labor>()[pos];
      return false;
    }

    public static bool CanWorkHere(IntVec3 pos, Pawn pawn, WorkTypeDef workType)
    {
      if (ShouldPawnBeConsidered(pawn) && pos != null && pawn.Map.areaManager.Get<Area_Labor>() != null &&
          !WorkSettings.WorkDisabled(workType))
      {
        bool result = true;
        try
        {
          result = !pawn.Map.areaManager.Get<Area_Labor>()[pos];
        }
        catch (IndexOutOfRangeException e)
        {
          DebugLogger.debug(
            $"{pawn.NameShortColored} cause IndexOutOfRangeException for {workType.label} calling pos {pos}");
        }

        return result;
      }

      return true;
    }

    private static bool ShouldPawnBeConsidered(Pawn pawn)
    {
      return pawn.IsFreeNonSlaveColonist || (!PrisonLaborPrefs.MechsWorkInLaborZone && pawn.IsColonyMech);
    }

    public static Faction GetPawnFaction(Pawn pawn)
    {
      return pawn.IsPrisonerOfColony ? Faction.OfPlayer : pawn.Faction;
    }

    public static bool CanUsePrisonerInteraction(this Pawn prisoner, PrisonerInteractionModeDef mode)
    {
      if (!prisoner.guest.Recruitable && mode.hideIfNotRecruitable)
      {
        return false;
      }

      if (prisoner.IsWildMan() && !mode.allowOnWildMan)
      {
        return false;
      }

      if (mode.hideIfNoBloodfeeders && prisoner.MapHeld != null && !ColonyHasAnyBloodfeeder(prisoner.MapHeld))
      {
        return false;
      }

      if (mode.hideOnHemogenicPawns && ModsConfig.BiotechActive && prisoner.genes != null &&
          prisoner.genes.HasActiveGene(GeneDefOf.Hemogenic))
      {
        return false;
      }

      if (!mode.allowInClassicIdeoMode && Find.IdeoManager.classicMode)
      {
        return false;
      }

      if (ModsConfig.AnomalyActive)
      {
        if (mode.hideIfNotStudiableAsPrisoner && !IsSuitableMethod.GetValue<bool>(prisoner))
        {
          return false;
        }

        if (mode.hideIfGrayFleshNotAppeared && !Find.Anomaly.hasSeenGrayFlesh)
        {
          return false;
        }
      }

      return true;
    }

    private static bool ColonyHasAnyBloodfeeder(Map map)
    {
      return ModsConfig.BiotechActive && map.mapPawns.FreeAdultColonistsSpawned.Any(pawn => pawn.IsBloodfeeder());
    }

    public static bool ColonyHasAnyWardenCapableOfViolence(Map map)
    {
      return map.mapPawns.FreeColonistsSpawned.Any(pawn =>
        pawn.workSettings.WorkIsActive(WorkTypeDefOf.Warden) && !pawn.WorkTagIsDisabled(WorkTags.Violent));
    }

    public static bool ColonyHasAnyWardenCapableOfEnslavement(Map map)
    {
      foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
      {
        if (item.workSettings.WorkIsActive(WorkTypeDefOf.Warden) &&
            new HistoryEvent(HistoryEventDefOf.EnslavedPrisoner, item.Named(HistoryEventArgsNames.Doer))
              .DoerWillingToDo())
        {
          return true;
        }
      }

      return false;
    }
  }
}