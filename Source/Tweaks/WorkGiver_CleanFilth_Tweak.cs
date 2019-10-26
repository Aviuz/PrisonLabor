using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Tweaks
{
    internal class WorkGiver_CleanFilth_Tweak : WorkGiver_Scanner
    {
        private readonly int MinTicksSinceThickened = 600;

        public override PathEndMode PathEndMode => PathEndMode.OnCell;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Filth);

        public override int LocalRegionsToScanFirst => 4;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerFilthInHomeArea.FilthInHomeArea;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (pawn.Faction != Faction.OfPlayer && !pawn.IsPrisoner)
                return false;
            var filth = t as Filth;
            return filth != null && filth.Map.areaManager.Home[filth.Position] &&
                   pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1, -1, null, forced) &&
                   filth.TicksSinceThickened >= MinTicksSinceThickened;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var job = new Job(JobDefOf.Clean);
            job.AddQueuedTarget(TargetIndex.A, t);
            var num = 15;
            var map = t.Map;
            var room = t.GetRoom(RegionType.Set_Passable);
            for (var i = 0; i < 100; i++)
            {
                var intVec = t.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && intVec.GetRoom(map, RegionType.Set_Passable) == room)
                {
                    var thingList = intVec.GetThingList(map);
                    for (var j = 0; j < thingList.Count; j++)
                    {
                        var thing = thingList[j];
                        if (HasJobOnThing(pawn, thing, forced) && thing != t)
                            job.AddQueuedTarget(TargetIndex.A, thing);
                    }
                    if (job.GetTargetQueue(TargetIndex.A).Count >= num)
                        break;
                }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 5)
                job.targetQueueA.SortBy(targ => targ.Cell.DistanceToSquared(pawn.Position));
            return job;
        }
    }
}