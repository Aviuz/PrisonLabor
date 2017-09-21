using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    internal class JobDriver_Supervise : JobDriver
    {
        private static readonly float GapLengh = 3.0f;

        protected Pawn Prisoner => (Pawn) CurJob.targetA.Thing;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnMentalState(TargetIndex.A);
            this.FailOnNotAwake(TargetIndex.A);
            this.FailOn(() => !Prisoner.IsPrisonerOfColony || !Prisoner.guest.PrisonerIsSecure);

            var rangeCondition = new System.Func<Toil, bool>(RangeCondition);

            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return MakeWatchToil(Prisoner);
            for (var i = 0; i < 80; i++)
                yield return Toils_General.Wait(10).FailOn(rangeCondition);
            yield return MakeWatchToil(Prisoner);
            for (var i = 0; i < 80; i++)
                yield return Toils_General.Wait(10).FailOn(rangeCondition);
            yield return Toils_Interpersonal.SetLastInteractTime(TargetIndex.A);
        }

        protected Toil MakeWatchToil(Pawn prisoner)
        {
            var toil = new Toil();
            toil.initAction = delegate
            {
                var actor = toil.actor;
                IntVec3 ind;
                if (Prisoner.GetRoom().Cells.Any(cell =>
                    cell.DistanceTo(Prisoner.InteractionCell) < Need_Motivation.InpirationRange - GapLengh &&
                    cell.DistanceTo(Prisoner.InteractionCell) > GapLengh))
                    ind = prisoner.GetRoom().Cells.Where(cell =>
                        cell.DistanceTo(prisoner.InteractionCell) < Need_Motivation.InpirationRange - GapLengh &&
                        cell.DistanceTo(prisoner.InteractionCell) > GapLengh).RandomElement();
                else
                    ind = prisoner.GetRoom().Cells.RandomElement();
                actor.pather.StartPath(ind, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        private bool RangeCondition(Toil toil)
        {
            return toil.actor.Position.DistanceTo(Prisoner.Position) > Need_Motivation.InpirationRange;
        }
    }
}