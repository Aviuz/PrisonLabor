using System;
using System.Collections.Generic;
using PrisonLabor.Core.LaborWorkSettings;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobGivers
{
    public class JobGiver_Labor : ThinkNode
    {
        public bool emergency;

        public object Tutorials { get; private set; }

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var jobGiver_Work = (JobGiver_Labor)base.DeepCopy(resolve);
            jobGiver_Work.emergency = emergency;
            return jobGiver_Work;
        }

        public override float GetPriority(Pawn pawn)
        {
            TimeAssignmentDef timeAssignmentDef = (pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment;
            if (timeAssignmentDef == TimeAssignmentDefOf.Joy)
            {
                DebugLogger.info($"[PL] Prisoner {pawn.NameShortColored} labor piority: 0");
                return 0f;
            }
            if (timeAssignmentDef == TimeAssignmentDefOf.Work)
            {
                DebugLogger.info($"[PL] Prisoner {pawn.NameShortColored} labor piority: 10");
                return 10f;
            }
            DebugLogger.info($"[PL] Prisoner {pawn.NameShortColored} labor piority: 8");
            return 8f;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            Need_Motivation need = pawn.needs.TryGetNeed<Need_Motivation>();

            if (NotHealthyEnoughToWork(pawn))
            {
                DebugLogger.debug($"Prisoner : {pawn.NameShortColored} is not working because is not healthy enough!");
                return NoWork(need);
            }
            if (ShouldNotWorkNow(pawn, need))
            {
                DebugLogger.debug($"Prisoner : {pawn.NameShortColored} is not working because of work hours or motivation!");
                return NoWork(need);
            }

            var workList = pawn.workSettings.WorkGiversInOrderNormal;
            var num = -999;
            var targetInfo = TargetInfo.Invalid;
            WorkGiver_Scanner workGiver_Scanner = null;
            for (var j = 0; j < workList.Count; j++)
            {
                var workGiver = workList[j];
                if (workGiver.def.priorityInType != num && targetInfo.IsValid)
                    break;
                if (PawnCanUseWorkGiver(pawn, workGiver))
                {
                    try
                    {

                        var noScanJob = workGiver.NonScanJob(pawn);
                        if (noScanJob != null)
                        {
                            return GoWork(new ThinkResult(noScanJob, this, workList[j].def.tagToGive), need);
                        }
                        var scanner = workGiver as WorkGiver_Scanner;
                        if (scanner != null)
                        {
                            if (workGiver.def.scanThings)
                            {
                                TryGetJobOnThing(pawn, ref targetInfo, ref workGiver_Scanner, scanner);
                            }
                            if (workGiver.def.scanCells)
                            {
                                TryGetJobOnCell(pawn, ref targetInfo, ref workGiver_Scanner, scanner);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Concat(pawn, " threw exception in WorkGiver ", workGiver.def.defName, ": ",
                            ex.ToString()));
                    }

                    if (targetInfo.IsValid)
                    {
                        Job jobToReturn;
                        if (targetInfo.HasThing)
                        {
                            jobToReturn = workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing, false);
                        }
                        else
                        {
                            jobToReturn = workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell);
                        }
                        if (jobToReturn != null)
                        {
                            jobToReturn.workGiverDef = workGiver.def;
                            DebugLogger.debug($"Prisoner : {pawn.NameShortColored} found work: {jobToReturn.def.defName}");
                            return GoWork(new ThinkResult(jobToReturn, this, workList[j].def.tagToGive), need);
                        }
                        Log.ErrorOnce(
                            string.Concat(workGiver_Scanner, " provided target ", targetInfo,
                                " but yielded no actual job for pawn ", pawn,
                                ". The CanGiveJob and JobOnX methods may not be synchronized."), 6112651);
                    }
                    num = workGiver.def.priorityInType;
                }
            }
            DebugLogger.debug($"Prisoner : {pawn.NameShortColored} can't find any work!");
            return NoWork(need);
        }

        private static void TryGetJobOnCell(Pawn pawn, ref TargetInfo targetInfo, ref WorkGiver_Scanner workGiver_Scanner, WorkGiver_Scanner scanner)
        {
            var position = pawn.Position;
            var minDistance = 99999f;
            var maxPiority = -3.40282347E+38f;
            var prioritized = scanner.Prioritized;
            foreach (var current in scanner.PotentialWorkCellsGlobal(pawn))
            {
                var flag = false;
                float tempDistance = (current - position).LengthHorizontalSquared;
                if (prioritized)
                {
                    if (!current.IsForbiddenForPrisoner(pawn) && scanner.HasJobOnCell(pawn, current))
                    {
                        var priority = scanner.GetPriority(pawn, current);
                        if (priority > maxPiority || priority == maxPiority && tempDistance < minDistance)
                        {
                            flag = true;
                            maxPiority = priority;
                        }
                    }
                }
                else if (tempDistance < minDistance && !current.IsForbidden(pawn) &&
                         scanner.HasJobOnCell(pawn, current))
                {
                    flag = true;
                }
                if (flag)
                {
                    targetInfo = new TargetInfo(current, pawn.Map, false);
                    workGiver_Scanner = scanner;
                    minDistance = tempDistance;
                }
            }
        }

        private void TryGetJobOnThing(Pawn pawn, ref TargetInfo targetInfo, ref WorkGiver_Scanner workGiver_Scanner, WorkGiver_Scanner scanner)
        {
            Predicate<Thing> predicate = t =>
                !t.IsForbiddenForPrisoner(pawn) && scanner.HasJobOnThing(pawn, t, false);
            var enumerable = scanner.PotentialWorkThingsGlobal(pawn);
            Thing thing;
            if (scanner.Prioritized)
            {
                var enumerable2 = enumerable;
                if (enumerable2 == null)
                    enumerable2 =
                        pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
                var validator = predicate;
                thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map,
                    enumerable2, scanner.PathEndMode,
                    TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f,
                    validator, x => scanner.GetPriority(pawn, x));
            }
            else
            {
                var validator = predicate;
                var forceGlobalSearch = enumerable != null;
                thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                    scanner.PotentialWorkThingRequest, scanner.PathEndMode,
                    TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f,
                    validator, enumerable, 0, scanner.MaxRegionsToScanBeforeGlobalSearch, forceGlobalSearch,
                    RegionType.Set_Passable, false);
            }
            if (thing != null)
            {
                targetInfo = thing;
                workGiver_Scanner = scanner;
            }
        }

        private bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
        {
            return (pawn.story == null || !pawn.WorkTagIsDisabled(giver.def.workTags)) &&
                   !giver.ShouldSkip(pawn) && (giver.def.nonColonistsCanDo || pawn.IsPrisoner) &&
                   giver.MissingRequiredCapacity(pawn) == null;
        }

        private ThinkResult GoWork(ThinkResult thinkResult, Need_Motivation need)
        {
            if (need != null)
                need.IsPrisonerWorking = true;
            return thinkResult;
        }

        private ThinkResult NoWork(Need_Motivation need)
        {
            if (need != null)
                need.IsPrisonerWorking = false;
            return ThinkResult.NoJob;
        }

        private bool NotHealthyEnoughToWork(Pawn pawn)
        {
            return HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn) ||
                PrisonLaborPrefs.EnableFullHealRest && (HealthAIUtility.ShouldBeTendedNowByPlayer(pawn) || HealthAIUtility.ShouldSeekMedicalRest(pawn));
        }

        private bool ShouldNotWorkNow(Pawn pawn, Need_Motivation need)
        {
            WorkSettings.InitWorkSettings(pawn);
            return !PrisonLaborUtility.WorkTime(pawn) || PrisonLaborPrefs.EnableMotivationMechanics && (need == null || need.IsLazy);
        }
    }
}