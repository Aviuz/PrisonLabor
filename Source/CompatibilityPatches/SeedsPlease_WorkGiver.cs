using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.CompatibilityPatches
{
    public class SeedsPlease_WorkGiver : WorkGiver_GrowerSow_Tweak
    {
        private const int SEEDS_TO_CARRY = 25;

        public override Job JobOnCell(Pawn pawn, IntVec3 c)
        {
            Job job = base.JobOnCell(pawn, c);
            if (job == null || job.plantDefToSow == null || job.plantDefToSow.blueprintDef == null)
            {
                return job;
            }
            Zone zone = GridsUtility.GetZone(c, pawn.Map);
            if (zone != null)
            {
                foreach (IntVec3 current in GenAdj.AdjacentCells8WayRandomized())
                {
                    IntVec3 intVec = c + current;
                    if (zone.ContainsCell(intVec))
                    {
                        foreach (Thing current2 in pawn.Map.thingGrid.ThingsAt(intVec))
                        {
                            if (current2.def != job.plantDefToSow && current2.def.BlockPlanting && ReservationUtility.CanReserve(pawn, current2, 1, -1, null, false) && !ForbidUtility.IsForbidden(current2, pawn))
                            {
                                if (current2.def.category == ThingCategory.Plant)
                                {
                                    Job result = new Job(JobDefOf.CutPlant, current2);
                                    return result;
                                }
                                if (current2.def.EverHaulable)
                                {
                                    Job result = HaulAIUtility.HaulAsideJobFor(pawn, current2);
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            Predicate<Thing> predicate = (Thing tempThing) => !ForbidUtility.IsForbidden(tempThing, pawn.Faction) && PawnLocalAwareness.AnimalAwareOf(pawn, tempThing) && ReservationUtility.CanReserve(pawn, tempThing, 1, -1, null, false);
            Thing thing = GenClosest.ClosestThingReachable(c, pawn.Map, ThingRequest.ForDef(job.plantDefToSow.blueprintDef), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, 0, false), 9999f, predicate, null, 0, -1, false, RegionType.Set_Passable, false);
            if (thing != null)
            {
                return new Job(DefDatabase<JobDef>.GetNamed("SowWithSeeds"), c, thing)
                {
                    plantDefToSow = job.plantDefToSow,
                    count = 25
                };
            }
            return null;
        }
    }
}
