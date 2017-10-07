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
                var prisonersInRoom = PrisonersInRoom(prisoner.GetRoom());
                IntVec3 ind = new IntVec3();
                int score = 0;
                int curScore = 0;
                bool found = false;
                foreach(var cell in prisoner.GetRoom().Cells)
                {
                    float distance = cell.DistanceTo(prisoner.InteractionCell);
                    if (distance < Need_Motivation.InpirationRange)
                    {
                        if (distance < GapLengh)
                            curScore = (int)distance;
                        else
                            curScore = (int)(Need_Motivation.InpirationRange - distance);

                        foreach (var pawn in prisonersInRoom)
                            if (cell.DistanceTo(pawn.Position) < Need_Motivation.InpirationRange)
                                curScore += 100;

                        if (curScore > score)
                        {
                            ind = cell;
                            score = curScore;
                            found = true;
                        }
                    }
                }
                if (!found)
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

        private IEnumerable<Pawn> PrisonersInRoom(Room room)
        {
            foreach(var pawn in room.Map.mapPawns.PrisonersOfColony)
            {
                if (pawn.GetRoom() == room)
                    yield return pawn;
            }
        }
    }
}