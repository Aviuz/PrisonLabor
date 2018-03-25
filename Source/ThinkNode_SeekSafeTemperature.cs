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
                var need = pawn.needs.TryGetNeed<Need_Motivation>();
                if (need == null)
                    return true;
                if (need.Inspired && PrisonLaborUtility.WorkTime(pawn))
                    return false;
                return true;
            }
            return false;
        }
    }
}