using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public class JobGiver_Labor : ThinkNode
    {
        public bool emergency;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_Labor jobGiver_Work = (JobGiver_Labor)base.DeepCopy(resolve);
            jobGiver_Work.emergency = this.emergency;
            return jobGiver_Work;
        }

        public override float GetPriority(Pawn pawn)
        {
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
                return 10f;
            else
                return 8f;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            if(pawn.timetable == null)
            {
                PrisonLaborUtility.InitWorkSettings(pawn);
            }
            if(pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy || pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Sleep)
            {
                pawn.needs.TryGetNeed<Need_Motivation>().Enabled = false;
                return ThinkResult.NoJob;
            }
            //Check medical assistance, fed, and rest if not override
            if(pawn.timetable.CurrentAssignment != TimeAssignmentDefOf.Work)
            {
                if (HealthAIUtility.ShouldSeekMedicalRest(pawn) || pawn.needs.food.CurCategory != HungerCategory.Fed || pawn.needs.rest.CurCategory != RestCategory.Rested)
                {
                    pawn.needs.TryGetNeed<Need_Motivation>().Enabled = false;
                    return ThinkResult.NoJob;
                }
            }
            //Check laziness
            if (pawn.needs.TryGetNeed<Need_Motivation>().IsLazy)
            {
                return ThinkResult.NoJob;
            }
            //Work prisoners will do
            PrisonLaborUtility.InitWorkSettings(pawn);
            List<WorkGiver> workList = pawn.workSettings.WorkGiversInOrderNormal;
            workList.RemoveAll(workGiver => workGiver.def.defName == "GrowerSow");
            pawn.needs.TryGetNeed<Need_Motivation>().Enabled = false;

            int num = -999;
            TargetInfo targetInfo = TargetInfo.Invalid;
            WorkGiver_Scanner workGiver_Scanner = null;
            for (int j = 0; j < workList.Count; j++)
            {
                WorkGiver workGiver = workList[j];
                if (workGiver.def.priorityInType != num && targetInfo.IsValid)
                {
                    break;
                }
                if (this.PawnCanUseWorkGiver(pawn, workGiver))
                {
                    try
                    {
                        Job job2 = workGiver.NonScanJob(pawn);
                        if (job2 != null)
                        {
                            pawn.needs.TryGetNeed<Need_Motivation>().Enabled = true;
                            return new ThinkResult(job2, this, new JobTag?(workList[j].def.tagToGive));
                        }
                        WorkGiver_Scanner scanner = workGiver as WorkGiver_Scanner;
                        if (scanner != null)
                        {
                            if (workGiver.def.scanThings)
                            {
                                Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false);
                                IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
                                Thing thing;
                                if (scanner.Prioritized)
                                {
                                    IEnumerable<Thing> enumerable2 = enumerable;
                                    if (enumerable2 == null)
                                    {
                                        enumerable2 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
                                    }
                                    Predicate<Thing> validator = predicate;
                                    thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x));
                                }
                                else
                                {
                                    Predicate<Thing> validator = predicate;
                                    bool forceGlobalSearch = enumerable != null;
                                    thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, enumerable, 0, scanner.LocalRegionsToScanFirst, forceGlobalSearch, RegionType.Set_Passable, false);
                                }
                                if (thing != null)
                                {
                                    targetInfo = thing;
                                    workGiver_Scanner = scanner;
                                }
                            }
                            if (workGiver.def.scanCells)
                            {
                                IntVec3 position = pawn.Position;
                                float num2 = 99999f;
                                float num3 = -3.40282347E+38f;
                                bool prioritized = scanner.Prioritized;
                                foreach (IntVec3 current in scanner.PotentialWorkCellsGlobal(pawn))
                                {
                                    bool flag = false;
                                    float num4 = (float)(current - position).LengthHorizontalSquared;
                                    if (prioritized)
                                    {
                                        if (!current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current))
                                        {
                                            float priority = scanner.GetPriority(pawn, current);
                                            if (priority > num3 || (priority == num3 && num4 < num2))
                                            {
                                                flag = true;
                                                num3 = priority;
                                            }
                                        }
                                    }
                                    else if (num4 < num2 && !current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current))
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
                        Log.Error(string.Concat(new object[]
                        {
                            pawn,
                            " threw exception in WorkGiver ",
                            workGiver.def.defName,
                            ": ",
                            ex.ToString()
                        }));
                    }
                    finally
                    {
                    }
                    if (targetInfo.IsValid)
                    {
                        pawn.mindState.lastGivenWorkType = workGiver.def.workType;
                        Job job3;
                        if (targetInfo.HasThing)
                        {
                            job3 = workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing, false);
                        }
                        else
                        {
                            job3 = workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell);
                        }
                        if (job3 != null)
                        {
                            pawn.needs.TryGetNeed<Need_Motivation>().Enabled = true;
                            return new ThinkResult(job3, this, new JobTag?(workList[j].def.tagToGive));
                        }
                        Log.ErrorOnce(string.Concat(new object[]
                        {
                            workGiver_Scanner,
                            " provided target ",
                            targetInfo,
                            " but yielded no actual job for pawn ",
                            pawn,
                            ". The CanGiveJob and JobOnX methods may not be synchronized."
                        }), 6112651);
                    }
                    num = workGiver.def.priorityInType;
                }
            }
            return ThinkResult.NoJob;
        }

        private bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
        {
            return !giver.ShouldSkip(pawn) && (giver.def.canBeDoneByNonColonists || pawn.IsPrisoner) && (pawn.story == null || !pawn.story.WorkTagIsDisabled(giver.def.workTags)) && giver.MissingRequiredCapacity(pawn) == null;
        }

        private Job GiverTryGiveJobPrioritized(Pawn pawn, WorkGiver giver, IntVec3 cell)
        {
            if (!this.PawnCanUseWorkGiver(pawn, giver))
            {
                return null;
            }
            try
            {
                Job job = giver.NonScanJob(pawn);
                if (job != null)
                {
                    Job result = job;
                    return result;
                }
                WorkGiver_Scanner scanner = giver as WorkGiver_Scanner;
                if (scanner != null)
                {
                    if (giver.def.scanThings)
                    {
                        Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false);
                        List<Thing> thingList = cell.GetThingList(pawn.Map);
                        for (int i = 0; i < thingList.Count; i++)
                        {
                            Thing thing = thingList[i];
                            if (scanner.PotentialWorkThingRequest.Accepts(thing) && predicate(thing))
                            {
                                pawn.mindState.lastGivenWorkType = giver.def.workType;
                                Job result = scanner.JobOnThing(pawn, thing, false);
                                return result;
                            }
                        }
                    }
                    if (giver.def.scanCells && !cell.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, cell))
                    {
                        pawn.mindState.lastGivenWorkType = giver.def.workType;
                        Job result = scanner.JobOnCell(pawn, cell);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Concat(new object[]
                {
                    pawn,
                    " threw exception in GiverTryGiveJobTargeted on WorkGiver ",
                    giver.def.defName,
                    ": ",
                    ex.ToString()
                }));
            }
            return null;
        }
    }
}
