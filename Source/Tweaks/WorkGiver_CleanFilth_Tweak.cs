﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    class WorkGiver_CleanFilth_Tweak : WorkGiver_Scanner
    {
        private int MinTicksSinceThickened = 600;

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.OnCell;
            }
        }

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Filth);
            }
        }

        public override int LocalRegionsToScanFirst
        {
            get
            {
                return 4;
            }
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerFilthInHomeArea.FilthInHomeArea;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!pawn.IsPrisoner)
            {
                return false;
            }
            Filth filth = t as Filth;
            return filth != null && filth.Map.areaManager.Home[filth.Position] && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1, -1, null, forced) && filth.TicksSinceThickened >= this.MinTicksSinceThickened;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job job = new Job(JobDefOf.Clean);
            job.AddQueuedTarget(TargetIndex.A, t);
            int num = 15;
            Map map = t.Map;
            Room room = t.GetRoom(RegionType.Set_Passable);
            for (int i = 0; i < 100; i++)
            {
                IntVec3 intVec = t.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && intVec.GetRoom(map, RegionType.Set_Passable) == room)
                {
                    List<Thing> thingList = intVec.GetThingList(map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        Thing thing = thingList[j];
                        if (this.HasJobOnThing(pawn, thing, forced) && thing != t)
                        {
                            job.AddQueuedTarget(TargetIndex.A, thing);
                        }
                    }
                    if (job.GetTargetQueue(TargetIndex.A).Count >= num)
                    {
                        break;
                    }
                }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 5)
            {
                job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
            }
            return job;
        }
    }
}
