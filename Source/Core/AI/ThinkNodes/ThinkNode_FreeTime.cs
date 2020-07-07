using System;
using PrisonLabor.Core.LaborWorkSettings;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.ThinkNodes
{
    public class ThinkNode_FreeTime : ThinkNode
    {
        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            var bed = pawn.CurrentBed();

            if (bed == null || pawn.health.HasHediffsNeedingTend())
                return ThinkResult.NoJob;

            Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PrisonLabor_FreePrisonerTime"), bed.GetRoom().Cells.RandomElement());
            return new ThinkResult(job, this);
        }
    }
}
