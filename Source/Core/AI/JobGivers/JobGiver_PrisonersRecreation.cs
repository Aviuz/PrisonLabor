using PrisonLabor.Core.Other;
using PrisonLabor.Core.Recreation;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobGivers
{
    public class JobGiver_PrisonersRecreation : ThinkNode
    {
        private JoyGiverDef walkDef;
        protected virtual bool CanDoDuringMedicalRest => false;
        public override float GetPriority(Pawn pawn)
        {
            TimeAssignmentDef timeAssignmentDef = (pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment;
            if (pawn.IsPrisoner && timeAssignmentDef == TimeAssignmentDefOf.Joy)
            {
                DebugLogger.debug($"[PL] Prisoner {pawn.NameShortColored} joy piority: 10");
                return 10f;
            }
            return 0f;
        }
        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            if (!CanDoDuringMedicalRest && pawn.InBed() && HealthAIUtility.ShouldSeekMedicalRest(pawn))
            {
                DebugLogger.debug($"[PL] Prisoner {pawn.NameShortColored} no joy because of medic needs");
                return ThinkResult.NoJob;
            }
            PrisonerJoyDef prisonerJoyDef = DefDatabase<PrisonerJoyDef>.GetNamed("PrisonLabor_PrisonersJoy");
            //JoyGiverDef result = prisonerJoyDef.avaliableForPrisonersJoy.RandomElement();
            if (prisonerJoyDef.avaliableForPrisonersJoy.TryRandomElementByWeight(def => def.Worker.GetChance(pawn), out JoyGiverDef result))
            {
                DebugLogger.debug($"[PL] Prisoner {pawn.NameShortColored} checking joy: {result.defName}.");
                Job job = result.Worker?.TryGiveJob(pawn);
                if (job != null)
                {
                    DebugLogger.debug($"[PL] Prisoner {pawn.NameShortColored} joy from {result.defName}.");
                    return new ThinkResult(job, this);
                }
            }
            DebugLogger.debug($"[PL] Prisoner {pawn.NameShortColored} no joy found. Trying walk");
            Job walk = walkDef.Worker.TryGiveJob(pawn);
            if (walk != null)
            {
                DebugLogger.debug($"[PL] Prisoner {pawn.NameShortColored} going for a walk.");
                return new ThinkResult(walk, this);
            }
            DebugLogger.debug($"[PL] Prisoner {pawn.NameShortColored} walk failed. Returning no job.");
            return ThinkResult.NoJob;
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            walkDef = DefDatabase<JoyGiverDef>.GetNamed("PrisonerLabor_RecrationWalk");
        }
    }
}
