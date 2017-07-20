using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PrisonLabor
{
    class ThinkNode_IfLaborEnabled : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.IsPrisoner)
            {
                //show tutorial
                Tutorials.Introduction();
                if (pawn.guest.interactionMode == DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption"))
                {
                    //can't escape
                    IntVec3 c;
                    if (pawn.guest.PrisonerIsSecure && !RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                    {
                                    return true;
                    }
                    pawn.needs.TryGetNeed<Need_Motivation>().Enabled = false;
                }
            }
            return false;
        }
    }
}
