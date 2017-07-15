using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;

namespace PrisonLabor
{
    public class JobDriver_PlantHarvest_Tweak : JobDriver_PlantWork_Tweak
    {
        protected override void Init()
        {
            this.xpPerTick = 0.11f;
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            foreach (Toil toil in base.MakeNewToils())
            {
                yield return toil;
            }
            yield return Toils_General.RemoveDesignationsOnThing(TargetIndex.A, DesignationDefOf.HarvestPlant);
        }
    }
}