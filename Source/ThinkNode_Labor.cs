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

                if (PrisonLaborUtility.LaborEnabled(pawn))
                {
                    // can't escape
                    IntVec3 c;
                    if (pawn.guest.PrisonerIsSecure &&
                        !RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                        return true;
                    // can escape but won't nearby guards
                    var need = pawn.needs.TryGetNeed<Need_Motivation>();
                    if (need != null)
                        if (need.Motivated)
                            return true;
                        else
                            need.Enabled = false;
                }
            }
            return false;
        }
    }
}