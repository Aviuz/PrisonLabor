using System;
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
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
                return 10f;
            return 8f;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            var need = pawn.needs.TryGetNeed<Need_Motivation>();

            if (pawn.timetable == null)
                WorkSettings.InitWorkSettings(pawn);
            if (HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
                return ThinkResult.NoJob;
            //Check medical assistance, fed, and rest if not override
            if (!PrisonLaborUtility.WorkTime(pawn))
            {
                Other.Tutorials.Timetable();
                if (need != null)
                    need.IsPrisonerWorking = false;
                return ThinkResult.NoJob;
            }
            //Check motivation
            if (PrisonLaborPrefs.EnableMotivationMechanics && (need == null || need.IsLazy))
                return ThinkResult.NoJob;
            //Work prisoners will do
            WorkSettings.InitWorkSettings(pawn);
            var workList = pawn.workSettings.WorkGiversInOrderNormal;
            //TODO check this
            //workList.RemoveAll(workGiver => workGiver.def.defName == "GrowerSow");
            if (need != null)
                need.IsPrisonerWorking = false;

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
                        var job2 = workGiver.NonScanJob(pawn);
                        if (job2 != null)
                        {
                            if (need != null)
                                need.IsPrisonerWorking = true;
                            return new ThinkResult(job2, this, workList[j].def.tagToGive);
                        }
                        var scanner = workGiver as WorkGiver_Scanner;
                        if (scanner != null)
                        {
                            if (workGiver.def.scanThings)
                            {
                                Predicate<Thing> predicate = t =>
                                    !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false);
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
                            if (workGiver.def.scanCells)
                            {
                                var position = pawn.Position;
                                var num2 = 99999f;
                                var num3 = -3.40282347E+38f;
                                var prioritized = scanner.Prioritized;
                                foreach (var current in scanner.PotentialWorkCellsGlobal(pawn))
                                {
                                    var flag = false;
                                    float num4 = (current - position).LengthHorizontalSquared;
                                    if (prioritized)
                                    {
                                        if (!current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current))
                                        {
                                            var priority = scanner.GetPriority(pawn, current);
                                            if (priority > num3 || priority == num3 && num4 < num2)
                                            {
                                                flag = true;
                                                num3 = priority;
                                            }
                                        }
                                    }
                                    else if (num4 < num2 && !current.IsForbidden(pawn) &&
                                             scanner.HasJobOnCell(pawn, current))
                                    {
                                        flag = true;
                                    }
                                    if (flag)
                                    {
                                        targetInfo = new TargetInfo(current, pawn.Map, false);
                                        workGiver_Scanner = scanner;
                                        num2 = num4;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Concat(pawn, " threw exception in WorkGiver ", workGiver.def.defName, ": ",
                            ex.ToString()));
                    }
                    finally
                    {
                    }
                    if (targetInfo.IsValid)
                    {
                        // TODO this is probably not correct
                        pawn.mindState.lastJobTag = JobTag.MiscWork;
                        Job job3;
                        if (targetInfo.HasThing)
                            job3 = workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing, false);
                        else
                            job3 = workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell);
                        if (job3 != null)
                        {
                            if (need != null)
                                need.IsPrisonerWorking = true;
                            return new ThinkResult(job3, this, workList[j].def.tagToGive);
                        }
                        Log.ErrorOnce(
                            string.Concat(workGiver_Scanner, " provided target ", targetInfo,
                                " but yielded no actual job for pawn ", pawn,
                                ". The CanGiveJob and JobOnX methods may not be synchronized."), 6112651);
                    }
                    num = workGiver.def.priorityInType;
                }
            }
            return ThinkResult.NoJob;
        }

        private bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
        {
            return !giver.ShouldSkip(pawn) && (giver.def.nonColonistsCanDo || pawn.IsPrisoner) &&
                   (pawn.story == null || !pawn.WorkTagIsDisabled(giver.def.workTags)) &&
                   giver.MissingRequiredCapacity(pawn) == null;
        }
    }
}