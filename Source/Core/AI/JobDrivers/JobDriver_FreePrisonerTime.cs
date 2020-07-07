using System;
using System.Collections.Generic;
using System.Linq;
using PrisonLabor.Core.Needs;
using RimWorld;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobDrivers
{
    public class JobDriver_FreePrisonerTime : JobDriver
    {
        private Pawn prisoner => ((Pawn)TargetA);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);

            yield return MakeWatchToil();
            for (int i = 0; i < 40; i++)
                yield return Toils_General.Wait(5);

            yield return MakeWatchToil();
            for (int i = 0; i < 40; i++)
                yield return Toils_General.Wait(5);
        }

        protected Toil MakeWatchToil()
        {
            var toil = new Toil();
            toil.initAction = delegate
            {
                var actor = toil.actor;
                var room = actor.GetRoom();
                var ind = room.Cells.RandomElement();
                var nTreatment = actor.needs.TryGetNeed<Need_Treatment>();

                if (nTreatment != null)
                    nTreatment.CurLevelPercentage = Mathf.Min(nTreatment.CurLevelPercentage + 0.02f, 1.0f);
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
