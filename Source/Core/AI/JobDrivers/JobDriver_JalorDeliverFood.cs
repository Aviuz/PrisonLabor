using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobDrivers
{
    public class JobDriver_JalorDeliverFood : JobDriver_FoodDeliver
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            var toils = base.MakeNewToils().ToList();

            toils.RemoveLast();
            Toil toil2 = new Toil();
            toil2.initAction = delegate
            {
                pawn.carryTracker.TryDropCarriedThing(toil2.actor.jobs.curJob.targetC.Cell, ThingPlaceMode.Direct, out Thing food, placedAction: (a, b) =>
                {
                    a.SetForbidden(true);
                });
            };
            toil2.defaultCompleteMode = ToilCompleteMode.Instant;
            toils.Add(toil2);
            return toils.AsEnumerable();
        }
    }
}
