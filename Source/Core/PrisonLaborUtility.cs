using System;
using System.Collections.Generic;
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
        private static readonly List<PrisonerInteractionModeDef> workOptions = new List<PrisonerInteractionModeDef> {
            PL_DefOf.PrisonLabor_workOption, PL_DefOf.PrisonLabor_workAndRecruitOption , PL_DefOf.PrisonLabor_workAndConvertOption,
            PL_DefOf.PrisonLabor_workAndEnslaveOption, PL_DefOf.PrisonLabor_workAndBloodfeedOption, PL_DefOf.PrisonLabor_workAndHemogenFarmOption
        };

        public static bool LaborEnabled(this Pawn pawn)
        {
            return pawn.IsPrisoner && workOptions.Contains(pawn.guest.interactionMode);
        }

        public static bool RecruitInLaborEnabled(Pawn pawn)
        {
            if (pawn.guest.interactionMode == PL_DefOf.PrisonLabor_workAndRecruitOption && pawn.guest.ScheduledForInteraction)
            {
                return true;
            }

            return false;
        }

        public static bool ConvertInLaborEnabled(Pawn doer, Pawn prisoner)
        {
            if (prisoner.guest.interactionMode == PL_DefOf.PrisonLabor_workAndConvertOption && prisoner.guest.ScheduledForInteraction
                && prisoner.Ideo != doer.Ideo && doer.Ideo == prisoner.guest.ideoForConversion)
            {
                return true;
            }
            return false;
        }

        public static bool EnslaveInLaborEnabled(Pawn doer, Pawn prisoner)
        {
            if (prisoner.guest.interactionMode == PL_DefOf.PrisonLabor_workAndEnslaveOption && prisoner.guest.ScheduledForInteraction
                && new HistoryEvent(HistoryEventDefOf.EnslavedPrisoner, doer.Named(HistoryEventArgsNames.Doer)).Notify_PawnAboutToDo_Job())
            {
                return true;
            }
            return false;
        }
        public static bool WorkTime(Pawn pawn)
        {
            if (pawn.timetable == null)
                return true;
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
                return true;
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything)
            {
                if (HealthAIUtility.ShouldSeekMedicalRest(pawn) ||
                    pawn.health.hediffSet.HasTemperatureInjury(TemperatureInjuryStage.Serious) ||
                    CheckFoodNeed(pawn) ||
                    CheckRestNeed(pawn))
                    return false;
                else
                    return true;
            }
            return false;
        }

        private static bool CheckFoodNeed(Pawn pawn)
        {
            return pawn.needs != null && pawn.needs.food != null && pawn.needs.food.CurCategory > HungerCategory.Hungry;
        }

        private static bool CheckRestNeed(Pawn pawn)
        {
            return pawn.needs != null && pawn.needs.rest != null && pawn.needs.rest.CurCategory != RestCategory.Rested;
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
            if (pawn.IsFreeNonSlaveColonist && pos != null && pawn.Map.areaManager.Get<Area_Labor>() != null && !WorkSettings.WorkDisabled(workType))
            {
                bool result = true;
                try
                {
                    result = !pawn.Map.areaManager.Get<Area_Labor>()[pos];
                }
                catch (IndexOutOfRangeException e)
                {
                    DebugLogger.debug($"{pawn.NameShortColored} cause IndexOutOfRangeException for {workType.label} calling pos {pos}");
                }
                return result;
            }
            return true;
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
            if (mode.hideOnHemogenicPawns && ModsConfig.BiotechActive && prisoner.genes != null && prisoner.genes.HasGene(GeneDefOf.Hemogenic))
            {
                return false;
            }
            if (!mode.allowInClassicIdeoMode && Find.IdeoManager.classicMode)
            {
                return false;
            }
            return true;
        }

        private static bool ColonyHasAnyBloodfeeder(Map map)
        {
            if (ModsConfig.BiotechActive)
            {
                foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
                {
                    if (item.IsBloodfeeder())
                    {
                        return true;
                    }
                }
                foreach (Pawn item2 in map.mapPawns.PrisonersOfColony)
                {
                    if (item2.IsBloodfeeder())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HemogenFarmInteractionMode(PrisonerInteractionModeDef interaction)
        {
            return interaction == PrisonerInteractionModeDefOf.HemogenFarm || interaction == PL_DefOf.PrisonLabor_workAndHemogenFarmOption;
        }

        public static bool BloodFeedInteractionMode(PrisonerInteractionModeDef interaction)
        {
            return interaction == PrisonerInteractionModeDefOf.Bloodfeed || interaction == PL_DefOf.PrisonLabor_workAndBloodfeedOption;
        }
    }
}