using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(JobGiver_Work))]
    [HarmonyPatch("TryIssueJobPackage")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(JobIssueParams) })]
    class LaborForbidPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Call, typeof(LaborForbidPatch).GetMethod("TryIssueJobPackage"));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        public static ThinkResult TryIssueJobPackage(JobGiver_Work jobGiver, Pawn pawn, JobIssueParams jobParams)
        {
            if (jobGiver.emergency && pawn.mindState.priorityWork.IsPrioritized)
            {
                List<WorkGiverDef> workGiversByPriority = pawn.mindState.priorityWork.WorkType.workGiversByPriority;
                for (int i = 0; i < workGiversByPriority.Count; i++)
                {
                    WorkGiver worker = workGiversByPriority[i].Worker;
                    Job job = GiverTryGiveJobPrioritized(jobGiver, pawn, worker, pawn.mindState.priorityWork.Cell);
                    if (job != null)
                    {
                        job.playerForced = true;
                        return new ThinkResult(job, jobGiver, new JobTag?(workGiversByPriority[i].tagToGive));
                    }
                }
                pawn.mindState.priorityWork.Clear();
            }
            List<WorkGiver> list = jobGiver.emergency ? pawn.workSettings.WorkGiversInOrderEmergency : pawn.workSettings.WorkGiversInOrderNormal;
            int num = -999;
            TargetInfo targetInfo = TargetInfo.Invalid;
            WorkGiver_Scanner workGiver_Scanner = null;
            for (int j = 0; j < list.Count; j++)
            {
                WorkGiver workGiver = list[j];
                if (workGiver.def.priorityInType != num && targetInfo.IsValid)
                {
                    break;
                }
                if (!workGiver.ShouldSkip(pawn) && (workGiver.def.canBeDoneByNonColonists || pawn.IsColonist) && (pawn.story == null || !pawn.story.WorkTagIsDisabled(workGiver.def.workTags)) && workGiver.MissingRequiredCapacity(pawn) == null)
                {
                    try
                    {
                        Job job2 = workGiver.NonScanJob(pawn);
                        if (job2 != null)
                        {
                            return new ThinkResult(job2, jobGiver, new JobTag?(list[j].def.tagToGive));
                        }
                        WorkGiver_Scanner scanner = workGiver as WorkGiver_Scanner;
                        if (scanner != null)
                        {
                            if (workGiver.def.scanThings)
                            {
                                Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false) && !LaborExclusionUtility.IsDisabledByLabor(t.Position, pawn, scanner.def.workType);
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
                                    else if (num4 < num2 && !current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current) && !LaborExclusionUtility.IsDisabledByLabor(current, pawn, scanner.def.workType))
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
                            return new ThinkResult(job3, jobGiver, new JobTag?(list[j].def.tagToGive));
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

        private static Job GiverTryGiveJobPrioritized(JobGiver_Work jobGiver, Pawn pawn, WorkGiver giver, IntVec3 cell)
        {
            if (!giver.ShouldSkip(pawn) && (giver.def.canBeDoneByNonColonists || pawn.IsColonist) && (pawn.story == null || !pawn.story.WorkTagIsDisabled(giver.def.workTags)) && giver.MissingRequiredCapacity(pawn) == null)
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
                        Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false) && !LaborExclusionUtility.IsDisabledByLabor(t.Position, pawn, scanner.def.workType);
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
                    if (giver.def.scanCells && !cell.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, cell) && !LaborExclusionUtility.IsDisabledByLabor(cell, pawn, scanner.def.workType))
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
