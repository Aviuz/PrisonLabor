using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public class JobDriver_Mine_Tweak : JobDriver
    {
        public const int BaseTicksBetweenPickHits = 120;

        private const int BaseDamagePerPickHit = 80;

        private const float MinMiningSpeedForNPCs = 0.5f;

        private Effecter effecter;

        private int ticksToPickHit = -1000;

        private Thing MineTarget => CurJob.GetTarget(TargetIndex.A).Thing;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnCellMissingDesignation(TargetIndex.A, DesignationDefOf.Mine);
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            var mine = new Toil();
            mine.tickAction = delegate
            {
                var actor = mine.actor;
                var mineTarget = MineTarget;
                if (ticksToPickHit < -100)
                    ResetTicksToPickHit();
                if (actor.skills != null)
                    actor.skills.Learn(SkillDefOf.Mining, 0.11f, false);
                ticksToPickHit--;
                if (ticksToPickHit <= 0)
                {
                    var position = mineTarget.Position;
                    if (effecter == null)
                        effecter = EffecterDefOf.Mine.Spawn();
                    effecter.Trigger(actor, mineTarget);
                    var num = 80;
                    var mineable = mineTarget as Mineable;
                    if (mineable == null || mineTarget.HitPoints > num)
                    {
                        var actor2 = mine.actor;
                        var dinfo = new DamageInfo(DamageDefOf.Mining, num, -1f, actor2, null, null,
                            DamageInfo.SourceCategory.ThingOrUnknown);
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
                        ReadyForNextToil();
                        return;
                    }
                    ResetTicksToPickHit();
                }
            };
            mine.defaultCompleteMode = ToilCompleteMode.Never;
            mine.WithProgressBar(TargetIndex.A,
                () => 1f - (float) MineTarget.HitPoints / (float) MineTarget.MaxHitPoints, false, -0.5f);
            mine.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return mine;
        }

        private void ResetTicksToPickHit()
        {
            var num = pawn.GetStatValue(StatDefOf.MiningSpeed, true);
            if (num < 0.5f && pawn.Faction != Faction.OfPlayer)
                num = 0.5f;
            ticksToPickHit = (int) Math.Round(120f / num);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToPickHit, "ticksToPickHit", 0, false);
        }
    }
}