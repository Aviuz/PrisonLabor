using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Unity.Jobs.LowLevel.Unsafe;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobDrivers
{
    public class JobDriver_JalorDeliverFood : JobDriver_FoodDeliver
    {

        protected override IEnumerable<Toil> MakeNewToils()
        {
            var toils = base.MakeNewToils().ToList();

            toils.RemoveRange(toils.Count - 1, 1);

            Toil toil2 = new Toil();
            toil2.initAction = delegate
            {
                pawn.carryTracker.TryDropCarriedThing(toil2.actor.CurJob.targetC.Cell, ThingPlaceMode.Direct, out Thing food, placedAction: (a, b) =>
                {
                    a.SetForbidden(true);
                    if (((Pawn)TargetB).needs.food.CurLevelPercentage < 0.15)
                    {
                        var def = new JobDef
                        {
                            driverClass = typeof(JobDriver_Ingest),
                            collideWithPawns = false,
                            defName = "PrisonLabor_PrisonerEat",
                            label = "Eating",
                            reportString = "Having a meal",
                            description = "Having the meal delivered by the guards.",
                            playerInterruptible = true,
                            checkOverrideOnDamage = CheckJobOverrideOnDamageMode.Always,
                            suspendable = true,
                            alwaysShowWeapon = false,
                            neverShowWeapon = true,
                            casualInterruptible = true
                        };

                        var nJob = JobMaker.MakeJob(def, new LocalTargetInfo(a));
                        nJob.count = job.count;

                        nJob.playerForced = false;
                        ((Pawn)TargetB).jobs.TryTakeOrderedJob(nJob);
                    }
                });
            };

            toil2.defaultCompleteMode = ToilCompleteMode.Instant;

            toils.Add(toil2);
            return toils.AsEnumerable();
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetB, job);
        }
    }
}
