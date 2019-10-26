using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace PrisonLabor.Tweaks
{
    public abstract class JobDriver_PlantWork_Tweak : JobDriver
    {
        protected const TargetIndex PlantInd = TargetIndex.A;

        private float workDone;

        protected float xpPerTick;

        protected Plant Plant => (Plant) job.targetA.Thing;

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            Init();
            yield return Toils_JobTransforms.MoveCurrentTargetIntoQueue(TargetIndex.A);
            yield return Toils_Reserve.ReserveQueue(TargetIndex.A, 1, -1, null);
            var initExtractTargetFromQueue =
                Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A);
            yield return initExtractTargetFromQueue;
            yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A);
            var checkNextQueuedTarget = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
                .JumpIfDespawnedOrNullOrForbidden(TargetIndex.A, checkNextQueuedTarget);
            var cut = new Toil();
            cut.tickAction = delegate
            {
                var actor = cut.actor;
                if (actor.skills != null)
                    actor.skills.Learn(SkillDefOf.Plants, xpPerTick, false);
                var statValue = actor.GetStatValue(StatDefOf.PlantWorkSpeed, true);
                var num = statValue;
                var plant = Plant;
                workDone += num;
                if (workDone >= plant.def.plant.harvestWork)
                {
                    if (plant.def.plant.harvestedThingDef != null)
                        if (actor.RaceProps.Humanlike && plant.def.plant.harvestFailable &&
                            Rand.Value > actor.GetStatValue(StatDefOf.PlantHarvestYield, true))
                        {
                            var loc = (pawn.DrawPos + plant.DrawPos) / 2f;
                            MoteMaker.ThrowText(loc, Map, "TextMote_HarvestFailed".Translate(), 3.65f);
                        }
                        else
                        {
                            var num2 = plant.YieldNow();
                            if (num2 > 0)
                            {
                                var thing = ThingMaker.MakeThing(plant.def.plant.harvestedThingDef, null);
                                thing.stackCount = num2;
                                GenPlace.TryPlaceThing(thing, actor.Position, Map, ThingPlaceMode.Near, null);
                                actor.records.Increment(RecordDefOf.PlantsHarvested);
                            }
                        }
                    plant.def.plant.soundHarvestFinish.PlayOneShot(actor);
                    plant.PlantCollected();
                    workDone = 0f;
                    ReadyForNextToil();
                }
            };
            cut.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            cut.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            cut.defaultCompleteMode = ToilCompleteMode.Never;
            cut.WithEffect(EffecterDefOf.Harvest, TargetIndex.A);
            cut.WithProgressBar(TargetIndex.A, () => workDone / Plant.def.plant.harvestWork, true, -0.5f);
            cut.PlaySustainerOrSound(() => Plant.def.plant.soundHarvesting);
            yield return cut;
            yield return checkNextQueuedTarget;
            yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.A, initExtractTargetFromQueue);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref workDone, "workDone", 0f, false);
        }

        protected virtual void Init()
        {
        }
    }
}