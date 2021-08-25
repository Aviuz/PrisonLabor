using PrisonLabor.Core.Components;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.ThinkNodes
{
    internal class ThinkNode_Labor : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.IsPrisoner)
            {
                //show tutorials
                Tutorials.Introduction();
                Tutorials.Management();

                IntVec3 c;

                var need = pawn.needs.TryGetNeed<Need_Motivation>();
                if (need == null)
                {
                    if (!pawn.guest.PrisonerIsSecure || RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                        return false;
                    else if (PrisonLaborUtility.LaborEnabled(pawn))
                        return true;
                    else
                        return false;
                }

                // Prisoner will escape if get ready to run.
                // If he can run he will start ticking impatient, once complete he will get ready.
              
                var prisonerComp = pawn.TryGetComp<PrisonerComp>();
                if (pawn.guest.PrisonerIsSecure && RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                {
                    if (prisonerComp.escapeTracker.ReadyToEscape)
                        return false;
                    else
                        prisonerComp.escapeTracker.CanEscape = true;
                }
                else
                {
                    prisonerComp.escapeTracker.CanEscape = false;
                }


                if (PrisonLaborUtility.LaborEnabled(pawn))
                {
                    return true;
                }

                need.IsPrisonerWorking = false;
            }
            return false;
        }
    }
}