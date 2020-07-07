using System.Collections.Generic;
using System.Linq;
using PrisonLabor.Constants;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Trackers;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobDrivers
{
    internal class JobDriver_Supervise : JobDriver
    {
        private static readonly float GapLengh = 3.0f;

        protected Pawn Prisoner => (Pawn)job.targetA.Thing;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnMentalState(TargetIndex.A);
            this.FailOnNotAwake(TargetIndex.A);

            this.FailOn(() => !Prisoner.IsPrisonerOfColony || !Prisoner.guest.PrisonerIsSecure);

            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);

            yield return MakeWatchToil(Prisoner);
            for (var i = 0; i < 80; i++)
                yield return Toils_General.Wait(3);

            yield return MakeWatchToil(Prisoner);
            for (var i = 0; i < 80; i++)
                yield return Toils_General.Wait(3);

            yield return MakeWatchToil(Prisoner);
            for (var i = 0; i < 40; i++)
                yield return Toils_General.Wait(3);

            yield return Toils_General.Wait(3);
            yield return Toils_Interpersonal.SetLastInteractTime(TargetIndex.A);
        }

        protected Toil MakeWatchToil(Pawn prisoner)
        {
            var toil = new Toil();
            toil.initAction = delegate
            {
                var actor = toil.actor;
                var room = actor.GetRoom();

                var prisonersIDInRoom = Tracked.Prisoners[actor.GetRoom().ID];

                foreach (int id in prisonersIDInRoom)
                {
                    var prisonerComp = Tracked.pawnComps[id];
                    var needM = ((Pawn)prisonerComp.parent).needs.TryGetNeed<Need_Motivation>();
                    needM.CurLevelPercentage = Mathf.Min(1.0f, needM.CurLevelPercentage + 0.095f);
                }

                var ind = prisoner.GetRoom().Cells.RandomElement();
                actor.pather.StartPath(ind, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
    }
}