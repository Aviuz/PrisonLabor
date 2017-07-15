using System;
using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse.AI;
using Verse;

namespace PrisonLabor
{
    public class JobDriver_PlantCut_Tweak : JobDriver_PlantWork_Tweak
    {
        protected override void Init()
        {
            if (base.Plant.def.plant.harvestedThingDef != null && base.Plant.YieldNow() > 0)
            {
                this.xpPerTick = 0.11f;
            }
            else
            {
                this.xpPerTick = 0f;
            }
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            foreach (Toil toil in base.MakeNewToils())
            {
                yield return toil;
            }
            Toil toil2 = new Toil();
            toil2.initAction = delegate
            {
                Pawn actor = toil2.actor;
                Thing thing = actor.jobs.curJob.GetTarget(TargetIndex.A).Thing;
                if (!thing.Destroyed)
                {
                    thing.Destroy(DestroyMode.Vanish);
                }
            };
            yield return toil2;
        }
    }
}