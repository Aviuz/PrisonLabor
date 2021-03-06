using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.WorkUtils
{
    class ConstructionUtils
    {
        public static bool HasJobOnThingFixed(Pawn pawn, Thing t, bool forced)
        {
            Building building = t as Building;
            if (building == null)
            {
                return false;
            }
            if (!pawn.Map.listerBuildingsRepairable.Contains(Faction.OfPlayer, building))
            {
                return false;
            }
            if (!building.def.building.repairable)
            {
                return false;
            }
            if (t.Faction != Faction.OfPlayer)
            {
                return false;
            }
            if (!t.def.useHitPoints || t.HitPoints == t.MaxHitPoints)
            {
                return false;
            }
            if (pawn.IsPrisonerOfColony && !pawn.Map.areaManager.Home[t.Position])
            {
                JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans);
                return false;
            }
            if (!pawn.CanReserveAndReach(building, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1, -1, null, forced))
            {
                return false;
            }
            if (building.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) != null)
            {
                return false;
            }
            if (building.def.mineable && building.Map.designationManager.DesignationAt(building.Position, DesignationDefOf.Mine) != null)
            {
                return false;
            }
            if (building.IsBurning())
            {
                return false;
            }
            return true;
        }


        public static bool isPrisonerWork(Thing t, Pawn pawn)
        {
            return pawn.IsPrisonerOfColony && t.Faction == Faction.OfPlayer;
        }
    }
}
