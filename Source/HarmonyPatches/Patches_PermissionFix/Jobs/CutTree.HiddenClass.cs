using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_PermissionFix.Jobs
{
    partial class CutTree
    {
        public class MyIterator : IEnumerable, IEnumerable<Toil>, IEnumerator, IDisposable, IEnumerator<Toil>
        {
            private class MyInnerIterator
			{
				Toil cut;

            CutTree.MyIterator iterator;

            internal bool someMethod(Thing t)
            {
                return this.iterator.$this.Map.designationManager.DesignationOn(t, this.iterator.$this.RequiredDesignation) != null;
            }

            internal void someOtherMethod()
            {
                Pawn actor = this.cut.actor;
                if (actor.skills != null)
                {
                    actor.skills.Learn(SkillDefOf.Plants, this.iterator.$this.xpPerTick, false);
                }
                float statValue = actor.GetStatValue(StatDefOf.PlantWorkSpeed, true);
                float num = statValue;
                Plant plant = this.iterator.$this.Plant;
                num *= Mathf.Lerp(3.3f, 1f, plant.Growth);
                this.iterator.$this.workDone += num;
                if (this.iterator.$this.workDone >= plant.def.plant.harvestWork)
					{
                    if (plant.def.plant.harvestedThingDef != null)
                    {
                        if (actor.RaceProps.Humanlike && plant.def.plant.harvestFailable && Rand.Value > actor.GetStatValue(StatDefOf.PlantHarvestYield, true))
                        {
                            Vector3 loc = (this.iterator.$this.pawn.DrawPos + plant.DrawPos) / 2f;
                            MoteMaker.ThrowText(loc, this.iterator.$this.Map, "TextMote_HarvestFailed".Translate(), 3.65f);
                        }
                        else
                        {
                            int num2 = plant.YieldNow();
                            if (num2 > 0)
                            {
                                Thing thing = ThingMaker.MakeThing(plant.def.plant.harvestedThingDef, null);
                                thing.stackCount = num2;
                                if (actor.Faction != Faction.OfPlayer)
                                {
                                    thing.SetForbidden(true, true);
                                }
                                GenPlace.TryPlaceThing(thing, actor.Position, this.iterator.$this.Map, ThingPlaceMode.Near, null, null);
                                actor.records.Increment(RecordDefOf.PlantsHarvested);
                            }
                        }
                    }
                    plant.def.plant.soundHarvestFinish.PlayOneShot(actor);
                    plant.PlantCollected();
                    this.iterator.$this.workDone = 0f;
                    this.iterator.$this.ReadyForNextToil();
                    return;
                }
            }

            internal float someWeirdMethod()
            {
                return this.iterator.$this.workDone / this.iterator.$this.Plant.def.plant.harvestWork;
            }

            internal SoundDef someWeirdestMethod()
            {
                return this.iterator.$this.Plant.def.plant.soundHarvesting;
            }
        }

        Toil<initExtractTargetFromQueue> __0;

        Toil<gotoThing> __1;

        Toil<plantWorkDoneToil> __0;

        JobDriver_PlantWork $this;

			Toil $current;

			internal bool $disposing;

			internal int $PC;

			private JobDriver_PlantWork.MyIterator.MyInnerIterator $locvar0;

			private static Func<SkillDef> <>f__am$cache0;

			[DebuggerHidden]
        public MyIterator()
        {
        }

        public bool MoveNext()
        {
            uint num = (uint)this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0u:
                    this.$locvar0 = new JobDriver_PlantWork.MyIterator.< MakeNewToils > c__AnonStorey1();
                    this.$locvar0.iterator = this;
                    this.$this.Init();
                    this.$current = Toils_JobTransforms.MoveCurrentTargetIntoQueue(TargetIndex.A);
                    if (!this.$disposing)
					{
                        this.$PC = 1;
                    }
                    return true;
                case 1u:
                    this.< initExtractTargetFromQueue > __0 = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A, (this.$this.RequiredDesignation == null) ? null : new Func<Thing, bool>(this.$locvar0.<> m__0));
                    this.$current = this.< initExtractTargetFromQueue > __0;
                    if (!this.$disposing)
					{
                        this.$PC = 2;
                    }
                    return true;
                case 2u:
                    this.$current = Toils_JobTransforms.SucceedOnNoTargetInQueue(TargetIndex.A);
                    if (!this.$disposing)
					{
                        this.$PC = 3;
                    }
                    return true;
                case 3u:
                    this.$current = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A, true);
                    if (!this.$disposing)
					{
                        this.$PC = 4;
                    }
                    return true;
                case 4u:
                    this.< gotoThing > __1 = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).JumpIfDespawnedOrNullOrForbidden(TargetIndex.A, this.< initExtractTargetFromQueue > __0);
                    if (this.$this.RequiredDesignation != null)
					{
                        this.< gotoThing > __1.FailOnThingMissingDesignation(TargetIndex.A, this.$this.RequiredDesignation);
                    }
                    this.$current = this.< gotoThing > __1;
                    if (!this.$disposing)
					{
                        this.$PC = 5;
                    }
                    return true;
                case 5u:
                    {
                        this.$locvar0.cut = new Toil();
                        this.$locvar0.cut.tickAction = new Action(this.$locvar0.<> m__1);
                        this.$locvar0.cut.FailOnDespawnedNullOrForbidden(TargetIndex.A);
                        if (this.$this.RequiredDesignation != null)
					{
                            this.$locvar0.cut.FailOnThingMissingDesignation(TargetIndex.A, this.$this.RequiredDesignation);
                        }
                        this.$locvar0.cut.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
                        this.$locvar0.cut.defaultCompleteMode = ToilCompleteMode.Never;
                        this.$locvar0.cut.WithEffect(EffecterDefOf.Harvest, TargetIndex.A);
                        this.$locvar0.cut.WithProgressBar(TargetIndex.A, new Func<float>(this.$locvar0.<> m__2), true, -0.5f);
                        this.$locvar0.cut.PlaySustainerOrSound(new Func<SoundDef>(this.$locvar0.someWeirdestMethod));
                        Toil arg_292_0 = this.$locvar0.cut;
                        if (JobDriver_PlantWork.MyIterator.<> f__am$cache0 == null)
					{
                            JobDriver_PlantWork.MyIterator.<> f__am$cache0 = new Func<SkillDef>(JobDriver_PlantWork.MyIterator.<> m__0);
                        }
                        arg_292_0.activeSkill = JobDriver_PlantWork.MyIterator.<> f__am$cache0;
                        this.$current = this.$locvar0.cut;
                        if (!this.$disposing)
					{
                            this.$PC = 6;
                        }
                        return true;
                    }
                case 6u:
                    this.< plantWorkDoneToil > __0 = this.$this.PlantWorkDoneToil();
                    if (this.< plantWorkDoneToil > __0 != null)
                    {
                        this.$current = this.< plantWorkDoneToil > __0;
                        if (!this.$disposing)
						{
                            this.$PC = 7;
                        }
                        return true;
                    }
                    break;
                case 7u:
                    break;
                case 8u:
                    this.$PC = -1;
                    return false;
                default:
                    return false;
            }
            this.$current = Toils_Jump.Jump(this.< initExtractTargetFromQueue > __0);
            if (!this.$disposing)
				{
                this.$PC = 8;
            }
            return true;
        }

        [DebuggerHidden]
        Toil IEnumerator<Toil>.get_Current()
        {
            return this.$current;
        }

        [DebuggerHidden]
        object IEnumerator.get_Current()
        {
            return this.$current;
        }

        [DebuggerHidden]
        public void Dispose()
        {
            this.$disposing = true;
            this.$PC = -1;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.System.Collections.Generic.IEnumerable<Verse.AI.Toil>.GetEnumerator();
        }

        [DebuggerHidden]
        IEnumerator<Toil> IEnumerable<Toil>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new JobDriver_PlantWork.MyIterator
            {
					$this = this.$this
            };
        }

        private static SkillDef<> m__0()
        {
            return SkillDefOf.Plants;
        }
    }
}
}
