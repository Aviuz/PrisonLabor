using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    class JobDriver_Supervise : JobDriver
    {
        private static float GapLengh = 3.0f;

        protected Pawn Prisoner
        {
            get
            {
                return (Pawn)base.CurJob.targetA.Thing;
            }
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnMentalState(TargetIndex.A);
            this.FailOnNotAwake(TargetIndex.A);
            this.FailOn(() => !Prisoner.IsPrisonerOfColony || !Prisoner.guest.PrisonerIsSecure);

            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            //yield return Toils_Interpersonal.GotoPrisoner(this.pawn, this.Prisoner, this.Prisoner.guest.interactionMode);
            yield return MakeWatchToil(Prisoner);
            for(int i = 0; i < 80; i++)
                yield return Toils_General.Wait(10).FailOn(() => Prisoner.GetRoom() != pawn.GetRoom());
            yield return MakeWatchToil(Prisoner);
            for (int i = 0; i < 80; i++)
                yield return Toils_General.Wait(10).FailOn(() => Prisoner.GetRoom() != pawn.GetRoom());
            yield return Toils_Interpersonal.SetLastInteractTime(TargetIndex.A);
        }

        protected Toil MakeWatchToil(Pawn prisoner)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                IntVec3 ind;
                if (Prisoner.GetRoom().Cells.Any(cell => cell.DistanceTo(Prisoner.InteractionCell) < Need_Motivation.InpirationRange - GapLengh && cell.DistanceTo(Prisoner.InteractionCell) > GapLengh))
                    ind = prisoner.GetRoom().Cells.Where(cell => cell.DistanceTo(prisoner.InteractionCell) < Need_Motivation.InpirationRange - GapLengh && cell.DistanceTo(prisoner.InteractionCell) > GapLengh).RandomElement();
                else
                    ind = prisoner.GetRoom().Cells.RandomElement();
                actor.pather.StartPath(ind, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }
    }
}
