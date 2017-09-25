using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public class WorkGiver_GrowerHarvest_Tweak : WorkGiver_Grower
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnCell(Pawn pawn, IntVec3 c)
        {
            if (!pawn.IsPrisoner)
                return false;
            var plant = c.GetPlant(pawn.Map);
            return plant != null && !plant.IsForbidden(pawn) && plant.def.plant.Harvestable &&
                   plant.LifeStage == PlantLifeStage.Mature && pawn.CanReserve(plant, 1, -1, null, false);
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 c)
        {
            var job = new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_Harvest_Tweak"));
            var map = pawn.Map;
            var room = c.GetRoom(map, RegionType.Set_Passable);
            var num = 0f;
            for (var i = 0; i < 40; i++)
            {
                var c2 = c + GenRadial.RadialPattern[i];
                if (c.GetRoom(map, RegionType.Set_Passable) == room)
                    if (HasJobOnCell(pawn, c2))
                    {
                        var plant = c2.GetPlant(map);
                        num += plant.def.plant.harvestWork;
                        if (num > 2400f)
                            break;
                        job.AddQueuedTarget(TargetIndex.A, plant);
                    }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 3)
                job.targetQueueA.SortBy(targ => targ.Cell.DistanceToSquared(pawn.Position));
            return job;
        }
    }
}