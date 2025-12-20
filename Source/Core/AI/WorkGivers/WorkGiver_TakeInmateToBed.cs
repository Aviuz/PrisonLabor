using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.WorkGivers
{
  public class WorkGiver_TakeInmateToBed : WorkGiver_Warden_TakeToBed
  {
    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
      return !pawn.IsPrisonerOfColony || base.ShouldSkip(pawn, forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
      return !pawn.IsPrisonerOfColony ? null : base.JobOnThing(pawn, t, forced);
    }
  }
}