using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PrisonLabor.Tweaks
{
    public class JobDriver_PlantCut_Tweak : JobDriver_PlantWork_Tweak
    {
        protected override void Init()
        {
            if (Plant.def.plant.harvestedThingDef != null && Plant.YieldNow() > 0)
                xpPerTick = 0.11f;
            else
                xpPerTick = 0f;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            foreach (var toil in base.MakeNewToils())
                yield return toil;
            var toil2 = new Toil();
            toil2.initAction = delegate
            {
                var actor = toil2.actor;
                var thing = actor.jobs.curJob.GetTarget(TargetIndex.A).Thing;
                if (!thing.Destroyed)
                    thing.Destroy(DestroyMode.Vanish);
            };
            yield return toil2;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
    }
}