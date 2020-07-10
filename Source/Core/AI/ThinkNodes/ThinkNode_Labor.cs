using System;
using System.Collections.Generic;
using System.Linq;
using PrisonLabor.Core.LaborArea;
using PrisonLabor.Core.LaborWorkSettings;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using RimWorld;
using UnityEngine.Assertions.Must;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.ThinkNodes
{
    public class ThinkNode_Labor : ThinkNode
    {
        public bool emergency;

        public object Tutorials { get; private set; }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            var mneed = pawn.needs.TryGetNeed<Need_Motivation>();

            // Check for medical needs
            if (HasMedicalNeed(pawn))
                return SetNotWorking(mneed);

            // Check the timetables in the worktab 
            if (!HasWorkTimeAssignment(pawn))
                return SetNotWorking(mneed);

            // Check if the motivation is enabled
            if (PrisonLaborPrefs.EnableMotivationMechanics && (mneed == null || mneed.IsLazy))
                return SetNotWorking(mneed);

            // TODO Remove this..
            // Work prisoners will do]
            if (pawn.timetable == null)
            {
                WorkSettings.InitWorkSettings(pawn);

                // TODO: improve locks
                // Restrict pawn to labor area
                pawn.playerSettings.AreaRestriction = pawn.Map.areaManager.Get<Area_Labor>();
            }

            // Parssing the entier job list "those selected in work tab"
            var workList = pawn.workSettings.WorkGiversInOrderNormal;
            var targetInfo = TargetInfo.Invalid;

            Job job = null;
            WorkGiver_Scanner scanner = null;

            // testing each job
            for (var j = 0; j < workList.Count; j++)
            {
                var workGiver = workList[j];

                // don't know what this does
                if (!PawnCanUseWorkGiver(pawn, workGiver))
                    continue;

                bool found = false;

                try
                {
                    // 3 types of job exsists:
                    // a. scan based
                    // b. cell based
                    // c. "nonScan"
                    job = workGiver.NonScanJob(pawn);
                    if (job != null)
                        return SetWorking(new ThinkResult(job, this, workList[j].def.tagToGive), mneed);

                    //A workscanner can help with finding the possible job related things or cells thus the name scanner
                    scanner = workGiver as WorkGiver_Scanner;
                    if (scanner == null)
                        continue;

                    Thing thing = null;
                    if (workGiver.def.scanThings)
                        thing = ScanThings(scanner, workGiver, pawn, ref targetInfo, ref found);

                    IntVec3 cell;
                    if (workGiver.def.scanCells && thing == null)
                        cell = ScanCells(scanner, workGiver, pawn, ref targetInfo, ref found);

                    if (!found)
                        continue;
                }
                catch (Exception e)
                {
#if TRACE
                    Log.Warning(e.Message);
                    Log.Warning(e.StackTrace);
                    Log.Warning(e.InnerException.StackTrace);
#endif
                    Log.Error(string.Concat(pawn, " threw exception in WorkGiver ", workGiver.def.defName, ": ",
                            e.ToString()));
                }

                if (!targetInfo.IsValid)
                    continue;

                if (targetInfo.HasThing)
                    job = scanner.JobOnThing(pawn, targetInfo.Thing, false);
                else
                    job = scanner.JobOnCell(pawn, targetInfo.Cell);

                if (job != null)
                    return SetWorking(new ThinkResult(job, this, workList[j].def.tagToGive), mneed);
            }


            return SetNotWorking(mneed);
        }

        /*
         This is used to scan for possible job related things, by "approximating" the distance to the target.
        example: Bills, haulling...
         */
        private Thing ScanThings(WorkGiver_Scanner scanner, WorkGiver workGiver, Pawn pawn, ref TargetInfo targetInfo, ref bool found)
        {
            Thing thing = null;

            IEnumerable<Thing> things = scanner.PotentialWorkThingsGlobal(pawn);

            if (things == null)
            {
                things = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
            }

            if (things != null)
            {
                float minDistance = 10e6f;

                // TODO Live with it
                // Aproximating "distance"
                foreach (Thing t in things)
                {
                    if (t == null)
                        continue;

                    // check if the shit is forbiden or not or is a subject of an other job
                    if (t.IsForbidden(pawn) || !scanner.HasJobOnThing(pawn, t, false))
                        continue;

                    if (!pawn.CanReach(new LocalTargetInfo(t.Position), PathEndMode.Touch, Danger.Unspecified))
                        continue;

                    var dist = pawn.Position.DistanceTo(t.Position);
                    if (dist <= minDistance)
                    {
                        minDistance = dist;
                        found = true;
                        thing = t;
                    }
                }
            }

            if (thing != null && found)
            {
                targetInfo = new TargetInfo(thing);
            }

            return thing;
        }

        /*
         This is used to scan for cells based jobs.
        example: Cleaning, etc...
         */
        private IntVec3 ScanCells(WorkGiver_Scanner scanner, WorkGiver workGiver, Pawn pawn, ref TargetInfo targetInfo, ref bool found)
        {
            IntVec3 cell = IntVec3.Zero;

            var position = pawn.Position;
            var minDistance = 99999f;
            var maxPriority = -3.40282347E+38f;
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
                        if (priority > maxPriority || priority == maxPriority && num4 < minDistance)
                        {
                            flag = true;
                            maxPriority = priority;
                        }
                    }
                }
                else if (num4 < minDistance && !current.IsForbidden(pawn) &&
                         scanner.HasJobOnCell(pawn, current))
                {
                    flag = true;
                }
                if (flag)
                {
                    targetInfo = new TargetInfo(current, pawn.Map, false);
                    found = true;

                    cell = current;
                    minDistance = num4;
                }
            }

            return cell;
        }

        private ThinkResult SetWorking(ThinkResult result, Need_Motivation motivation)
        {
            if (motivation != null && PrisonLaborPrefs.EnableMotivationMechanics)
                motivation.IsPrisonerWorking = true;
            return result;
        }

        private ThinkResult SetNotWorking(Need_Motivation motivation)
        {
            if (motivation != null && PrisonLaborPrefs.EnableMotivationMechanics)
                motivation.IsPrisonerWorking = false;
            return ThinkResult.NoJob;
        }

        private bool HasMedicalNeed(Pawn pawn)
        {
            if (HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
                return true;

            if (PrisonLaborPrefs.EnableFullHealRest && (pawn.health.HasHediffsNeedingTend()))
                return true;

            return false;
        }

        private bool HasWorkTimeAssignment(Pawn pawn)
        {
            if (pawn.timetable == null)
                WorkSettings.InitWorkSettings(pawn);

            if (PrisonLaborUtility.WorkTime(pawn))
                return true;

            return false;
        }

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            var jobGiver_Work = (ThinkNode_Labor)base.DeepCopy(resolve);
            jobGiver_Work.emergency = emergency;
            return jobGiver_Work;
        }

        public override float GetPriority(Pawn pawn)
        {
            if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work)
                return 10f;
            return 8f;
        }

        private bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
        {
            if (pawn.story != null && pawn.WorkTagIsDisabled(giver.def.workTags))
                return false;

            if (giver.ShouldSkip(pawn))
                return false;

            if (giver.MissingRequiredCapacity(pawn) != null)
                return false;

            return true;
        }
    }
}
