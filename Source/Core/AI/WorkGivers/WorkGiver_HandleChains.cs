using PrisonLabor.Constants;
using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.WorkGivers
{
    public class WorkGiver_HandleChains : WorkGiver_Warden
    {
         public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            CuffsTracker cuffsTracker = pawn.Map.GetComponent<CuffsTracker>();
            if(cuffsTracker == null)
            {
                return true;
            }
            return cuffsTracker != null && cuffsTracker.legscuffTracker.Count == 0 && cuffsTracker.handscuffTracker.Count == 0;
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.mapPawns.PrisonersOfColonySpawned;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!ShouldTakeCareOfPrisoner(pawn, t))
            {
                return null;
            }
            if (pawn.IsFreeNonSlaveColonist)
            {
                CuffsTracker cuffsTracker = pawn.Map.GetComponent<CuffsTracker>();
                if (cuffsTracker != null)
                {
                    Pawn prisoner = t as Pawn;
                    if (cuffsTracker.legscuffTracker.ContainsKey(prisoner))
                    {
                        return new Job(PL_DefOf.PrisonLabor_HandlePrisonersLegChain, prisoner);
                    }
                    if (cuffsTracker.handscuffTracker.ContainsKey(prisoner))
                    {
                        return new Job(PL_DefOf.PrisonLabor_HandlePrisonersHandChain, prisoner);
                    }
                }
            }
            return null;
        }
    }
}
