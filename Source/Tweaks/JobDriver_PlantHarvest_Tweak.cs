using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace PrisonLabor
{
    public class JobDriver_PlantHarvest_Tweak : JobDriver_PlantWork_Tweak
    {
        protected override void Init()
        {
            xpPerTick = 0.11f;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            foreach (var toil in base.MakeNewToils())
                yield return toil;
            yield return Toils_General.RemoveDesignationsOnThing(TargetIndex.A, DesignationDefOf.HarvestPlant);
        }

        public override bool TryMakePreToilReservations()
        {
            return true;
        }
    }
}