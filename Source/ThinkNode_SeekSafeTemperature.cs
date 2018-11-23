using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    internal class ThinkNode_SeekSafeTemperature : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (pawn.IsPrisoner)
            {
                if (pawn.IsWatched() && PrisonLaborUtility.WorkTime(pawn))
                    return false;
                return true;
            }
            return false;
        }
    }
}