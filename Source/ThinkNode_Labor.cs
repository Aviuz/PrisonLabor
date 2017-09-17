using RimWorld;
using Verse;

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

                var need = pawn.needs.TryGetNeed<Need_Motivation>();
                if (need == null)
                    return false;

                // Prisoner will escape if get ready to run.
                // If he can run he will start ticking impatient, once complete he will get ready.
                IntVec3 c;
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

                need.Enabled = false;
            }
            return false;
        }
    }
}