using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PrisonLabor
{
    class ThinkNode_ConditionalIsForced : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            //can work
            if (pawn.IsPrisoner)
            {
                //show tutorial
                Tutorials.PrisonLabor();
                if (pawn.guest.interactionMode == DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption"))
                {
                    //can't escape
                    IntVec3 c;
                    if (pawn.guest.PrisonerIsSecure && !RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
                    {
                        //shouldn't rest (medical reasons)
                        if (!HealthAIUtility.ShouldSeekMedicalRest(pawn))
                        {
                            // TODO can't or don't want to change
                            if (true)
                            {
                                // needs satisfied (sleep, food, etc.)
                                // TODO add more
                                if (pawn.needs.food.CurCategory == HungerCategory.Fed &&
                                    pawn.needs.rest.CurCategory == RestCategory.Rested)
                                {
                                    Laziness.tick(pawn);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
