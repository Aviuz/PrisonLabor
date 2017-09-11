using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public class WorkGiver_GrowerSow_Tweak : WorkGiver_Grower
    {
        public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

        protected override bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
        {
            if (!settable.CanAcceptSowNow())
                return false;
            var zone_Growing = settable as Zone_Growing;
            IntVec3 c;
            if (zone_Growing != null)
            {
                if (!zone_Growing.allowSow)
                    return false;
                c = zone_Growing.Cells[0];
            }
            else
            {
                c = ((Thing) settable).Position;
            }
            wantedPlantDef = CalculateWantedPlantDef(c, pawn.Map);
            return wantedPlantDef != null;
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 c)
        {
            if (!pawn.IsPrisoner)
                return null;
            if (c.IsForbidden(pawn))
                return null;
            if (!GenPlant.GrowthSeasonNow(c, pawn.Map))
                return null;
            if (wantedPlantDef == null)
            {
                wantedPlantDef = CalculateWantedPlantDef(c, pawn.Map);
                if (wantedPlantDef == null)
                    return null;
            }
            var thingList = c.GetThingList(pawn.Map);
            for (var i = 0; i < thingList.Count; i++)
            {
                var thing = thingList[i];
                if (thing.def == wantedPlantDef)
                    return null;
                if ((thing is Blueprint || thing is Frame) && thing.Faction == pawn.Faction)
                    return null;
            }
            var plant = c.GetPlant(pawn.Map);
            if (plant != null && plant.def.plant.blockAdjacentSow)
            {
                if (!pawn.CanReserve(plant, 1, -1, null, false) || plant.IsForbidden(pawn))
                    return null;
                return new Job(JobDefOf.CutPlant, plant);
            }
            var thing2 = GenPlant.AdjacentSowBlocker(wantedPlantDef, c, pawn.Map);
            if (thing2 != null)
            {
                var plant2 = thing2 as Plant;
                if (plant2 != null && pawn.CanReserve(plant2, 1, -1, null, false) && !plant2.IsForbidden(pawn))
                {
                    var plantToGrowSettable = plant2.Position.GetPlantToGrowSettable(plant2.Map);
                    if (plantToGrowSettable == null || plantToGrowSettable.GetPlantDefToGrow() != plant2.def)
                        return new Job(JobDefOf.CutPlant, plant2);
                }
                return null;
            }
            if (wantedPlantDef.plant.sowMinSkill > 0 && !PrisonLaborPrefs.AdvancedGrowing)
            {
                Tutorials.Growing();
                return null;
            }
            var j = 0;
            while (j < thingList.Count)
            {
                var thing3 = thingList[j];
                if (thing3.def.BlockPlanting)
                {
                    if (!pawn.CanReserve(thing3, 1, -1, null, false))
                        return null;
                    if (thing3.def.category == ThingCategory.Plant)
                    {
                        if (!thing3.IsForbidden(pawn))
                            return new Job(JobDefOf.CutPlant, thing3);
                        return null;
                    }
                    if (thing3.def.EverHaulable)
                        return HaulAIUtility.HaulAsideJobFor(pawn, thing3);
                    return null;
                }
                j++;
            }
            if (!wantedPlantDef.CanEverPlantAt(c, pawn.Map) || !GenPlant.GrowthSeasonNow(c, pawn.Map) ||
                !pawn.CanReserve(c, 1, -1, null, false))
                return null;
            return new Job(JobDefOf.Sow, c)
            {
                plantDefToSow = wantedPlantDef
            };
        }
    }
}