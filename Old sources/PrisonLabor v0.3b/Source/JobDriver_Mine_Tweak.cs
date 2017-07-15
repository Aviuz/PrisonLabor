using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public class JobDriver_Mine_Tweak : JobDriver
    {
        public const int BaseTicksBetweenPickHits = 120;

        private const int BaseDamagePerPickHit = 80;

        private const float MinMiningSpeedForNPCs = 0.5f;

        private int ticksToPickHit = -1000;

        private Effecter effecter;

        private Thing MineTarget
        {
            get
            {
                return base.CurJob.GetTarget(TargetIndex.A).Thing;
            }
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnCellMissingDesignation(TargetIndex.A, DesignationDefOf.Mine);
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil mine = new Toil();
            mine.tickAction = delegate
            {
                Pawn actor = mine.actor;
                Thing mineTarget = this.MineTarget;
                if (this.ticksToPickHit < -100)
                {
                    this.ResetTicksToPickHit();
                }
                if (actor.skills != null)
                {
                    actor.skills.Learn(SkillDefOf.Mining, 0.11f, false);
                }
                this.ticksToPickHit--;
                if (this.ticksToPickHit <= 0)
                {
                    IntVec3 position = mineTarget.Position;
                    if (this.effecter == null)
                    {
                        this.effecter = EffecterDefOf.Mine.Spawn();
                    }
                    this.effecter.Trigger(actor, mineTarget);
                    int num = 80;
                    Mineable mineable = mineTarget as Mineable;
                    if (mineable == null || mineTarget.HitPoints > num)
                    {
                        Pawn actor2 = mine.actor;
                        DamageInfo dinfo = new DamageInfo(DamageDefOf.Mining, num, -1f, actor2, null, null, DamageInfo.SourceCategory.ThingOrUnknown);
                        mineTarget.TakeDamage(dinfo);
                    }
                    else
                    {
                        mineable.DestroyMined(actor);
                    }
                    if (mineTarget.Destroyed)
                    {
                        actor.Map.mineStrikeManager.CheckStruckOre(position, mineTarget.def, actor);
                        actor.records.Increment(RecordDefOf.CellsMined);
                        this.ReadyForNextToil();
                        return;
                    }
                    this.ResetTicksToPickHit();
                }
            };
            mine.defaultCompleteMode = ToilCompleteMode.Never;
            mine.WithProgressBar(TargetIndex.A, () => 1f - (float)this.MineTarget.HitPoints / (float)this.MineTarget.MaxHitPoints, false, -0.5f);
            mine.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return mine;
        }

        private void ResetTicksToPickHit()
        {
            float num = this.pawn.GetStatValue(StatDefOf.MiningSpeed, true);
            if (num < 0.5f && this.pawn.Faction != Faction.OfPlayer)
            {
                num = 0.5f;
            }
            this.ticksToPickHit = (int)Math.Round((double)(120f / num));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToPickHit, "ticksToPickHit", 0, false);
        }
    }
}
