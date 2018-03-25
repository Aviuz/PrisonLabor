using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
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
                if (!pawn.guest.PrisonerIsSecure ||
                        RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                {
                    need.CanEscape = true;
                    if (need.ReadyToRun)
                        return false;
                }
                else
                {
                    need.CanEscape = false;
                }


                if (PrisonLaborUtility.LaborEnabled(pawn))
                {
                    return true;
                }

                need.PrisonerWorking = false;
            }
            return false;
        }
    }
}