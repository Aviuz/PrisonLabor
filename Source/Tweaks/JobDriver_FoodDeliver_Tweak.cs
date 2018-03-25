using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public class JobDriver_FoodDeliver_Tweak : JobDriver
    {
        private const TargetIndex FoodSourceInd = TargetIndex.A;

        private const TargetIndex DelivereeInd = TargetIndex.B;

        private bool eatingFromInventory;

        private bool usingNutrientPasteDispenser;

        private Pawn Deliveree => (Pawn) job.targetB.Thing;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref usingNutrientPasteDispenser, "usingNutrientPasteDispenser", false, false);
            Scribe_Values.Look(ref eatingFromInventory, "eatingFromInventory", false, false);
        }

        public override string GetReport()
        {
            if (job.GetTarget(TargetIndex.A).Thing is Building_NutrientPasteDispenser)
                return job.def.reportString.Replace("TargetA", ThingDefOf.MealNutrientPaste.label)
                    .Replace("TargetB", ((Pawn) (Thing) job.targetB).LabelShort);
            return base.GetReport();
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();
            usingNutrientPasteDispenser = TargetThingA is Building_NutrientPasteDispenser;
            eatingFromInventory = pawn.inventory != null && pawn.inventory.Contains(TargetThingA);
        }

        public override bool TryMakePreToilReservations()
        {
            return true;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => PrisonerFoodReservation.IsReserved(TargetA.Thing));
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            if (eatingFromInventory)
            {
                yield return Toils_Misc.TakeItemFromInventoryToCarrier(pawn, TargetIndex.A);
            }
            else if (usingNutrientPasteDispenser)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell)
                    .FailOnForbidden(TargetIndex.A);
                yield return Toils_Ingest.TakeMealFromDispenser(TargetIndex.A, pawn);
            }
            else
            {
                yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
                    .FailOnForbidden(TargetIndex.A);
                yield return Toils_Ingest.PickupIngestible(TargetIndex.A, Deliveree);
            }
            var toil = new Toil();
            toil.initAction = delegate
            {
                var actor = toil.actor;
                var curJob = actor.jobs.curJob;
                actor.pather.StartPath(curJob.targetC, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.FailOnDestroyedNullOrForbidden(TargetIndex.B);
            toil.AddFailCondition(delegate
            {
                var pawn = (Pawn) toil.actor.jobs.curJob.targetB.Thing;
                return !pawn.IsPrisonerOfColony || !pawn.guest.CanBeBroughtFood;
            });
            yield return toil;
            yield return new Toil
            {
                initAction = delegate
                {
                    Thing thing;
                    pawn.carryTracker.TryDropCarriedThing(toil.actor.jobs.curJob.targetC.Cell, ThingPlaceMode.Direct,
                        out thing, null);
                    PrisonerFoodReservation.reserve(thing, (Pawn) toil.actor.jobs.curJob.targetB.Thing);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}