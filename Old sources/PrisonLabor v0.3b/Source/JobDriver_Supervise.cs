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
                if (Prisoner.GetRoom().Cells.Any(cell => cell.DistanceTo(Prisoner.InteractionCell) < 7 && cell.DistanceTo(Prisoner.InteractionCell) > 4))
                    ind = prisoner.GetRoom().Cells.Where(cell => cell.DistanceTo(prisoner.InteractionCell) < 7 && cell.DistanceTo(prisoner.InteractionCell) > 4).RandomElement();
                else
                    ind = prisoner.GetRoom().Cells.RandomElement();
                actor.pather.StartPath(ind, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }
    }
}
