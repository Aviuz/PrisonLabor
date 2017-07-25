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
                if (PrisonLaborUtility.LaborEnabled(pawn))
                {
                    //can't escape
                    IntVec3 c;
                    if (pawn.guest.PrisonerIsSecure && !RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                    {
                        return true;
                    }
                    if(pawn.needs.TryGetNeed<Need_Motivation>() != null)
                        pawn.needs.TryGetNeed<Need_Motivation>().Enabled = false;
                }
            }
            return false;
        }
    }
}
