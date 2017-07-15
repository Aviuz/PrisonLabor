using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace PrisonLabor
{
    public abstract class JobDriver_PlantWork_Tweak : JobDriver
    {
        protected const TargetIndex PlantInd = TargetIndex.A;

        private float workDone;

        protected float xpPerTick;

        protected Plant Plant
        {
            get
            {
                return (Plant)base.CurJob.targetA.Thing;
            }
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.Init();
            yield return Toils_JobTransforms.MoveCurrentTargetIntoQueue(TargetIndex.A);
            yield return Toils_Reserve.ReserveQueue(TargetIndex.A, 1, -1, null);
            Toil initExtractTargetFromQueue = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A);
            yield return initExtractTargetFromQueue;
            yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A);
            Toil checkNextQueuedTarget = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).JumpIfDespawnedOrNullOrForbidden(TargetIndex.A, checkNextQueuedTarget);
            Toil cut = new Toil();
            cut.tickAction = delegate
            {
                Pawn actor = cut.actor;
                if (actor.skills != null)
                {
                    actor.skills.Learn(SkillDefOf.Growing, this.xpPerTick, false);
                }
                float statValue = actor.GetStatValue(StatDefOf.PlantWorkSpeed, true);
                float num = statValue;
                Plant plant = this.Plant;
                this.workDone += num;
                if (this.workDone >= plant.def.plant.harvestWork)
                {
                    if (plant.def.plant.harvestedThingDef != null)
                    {
                        if (actor.RaceProps.Humanlike && plant.def.plant.harvestFailable && Rand.Value > actor.GetStatValue(StatDefOf.PlantHarvestYield, true))
                        {
                            Vector3 loc = (this.pawn.DrawPos + plant.DrawPos) / 2f;
                            MoteMaker.ThrowText(loc, this.Map, "TextMote_HarvestFailed".Translate(), 3.65f);
                        }
                        else
                        {
                            int num2 = plant.YieldNow();
                            if (num2 > 0)
                            {
                                Thing thing = ThingMaker.MakeThing(plant.def.plant.harvestedThingDef, null);
                                thing.stackCount = num2;
                                GenPlace.TryPlaceThing(thing, actor.Position, this.Map, ThingPlaceMode.Near, null);
                                actor.records.Increment(RecordDefOf.PlantsHarvested);
                            }
                        }
                    }
                    plant.def.plant.soundHarvestFinish.PlayOneShot(actor);
                    plant.PlantCollected();
                    this.workDone = 0f;
                    this.ReadyForNextToil();
                    return;
                }
            };
            cut.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            cut.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            cut.defaultCompleteMode = ToilCompleteMode.Never;
            cut.WithEffect(EffecterDefOf.Harvest, TargetIndex.A);
            cut.WithProgressBar(TargetIndex.A, () => this.workDone / this.Plant.def.plant.harvestWork, true, -0.5f);
            cut.PlaySustainerOrSound(() => this.Plant.def.plant.soundHarvesting);
            yield return cut;
            yield return checkNextQueuedTarget;
            yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.A, initExtractTargetFromQueue);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.workDone, "workDone", 0f, false);
        }

        protected virtual void Init()
        {
        }
    }
}
