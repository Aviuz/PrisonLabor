using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public class WorkGiver_PlantsCut_Tweak : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            var desList = pawn.Map.designationManager.allDesignations;
            for (var i = 0; i < desList.Count; i++)
            {
                var des = desList[i];
                if (des.def == DesignationDefOf.CutPlant || des.def == DesignationDefOf.HarvestPlant)
                    yield return des.target.Thing;
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!pawn.IsPrisoner || !pawn.IsPrisonerOfColony)
                return null;
            if (t.def.category != ThingCategory.Plant)
                return null;
            if (!pawn.CanReserve(t, 1, -1, null, false))
                return null;
            if (t.IsForbidden(pawn))
                return null;
            if (t.IsBurning())
                return null;
            foreach (var current in pawn.Map.designationManager.AllDesignationsOn(t))
                if (current.def == DesignationDefOf.HarvestPlant)
                {
                    Job result;
                    if (!((Plant) t).HarvestableNow)
                    {
                        result = null;
                        return result;
                    }
                    result = new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_Harvest_Tweak"), t);
                    return result;
                }
                else if (current.def == DesignationDefOf.CutPlant)
                {
                    var result = new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor_CutPlant_Tweak"), t);
                    return result;
                }
            return null;
        }
    }
}