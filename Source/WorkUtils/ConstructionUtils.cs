using HarmonyLib;
using PrisonLabor.Core;
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
            if (!PawnCanRepairNow(pawn, t))
                return false;
            Building t1 = t as Building;
            if (PrisonLaborUtility.GetPawnFaction(pawn) == Faction.OfPlayer && !pawn.Map.areaManager.Home[t.Position])
            {
                JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans, null);
                return false;
            }
            return pawn.CanReserveAndReach(t1, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1, -1, null, forced) &&
                t1.Map.designationManager.DesignationOn(t1, DesignationDefOf.Deconstruct) == null &&
                (!t1.def.mineable || t1.Map.designationManager.DesignationAt(t1.Position, DesignationDefOf.Mine) == null) &&
                !t1.IsBurning();

        }


        public static bool IsPrisonerWork(Thing t, Pawn pawn)
        {
            return pawn.IsPrisonerOfColony && t.Faction == Faction.OfPlayer;
        }

        private static bool PawnCanRepairEver(Pawn pawn, Thing t)
        {
            return t is Building building && t.def.useHitPoints && (building.def.building.repairable && t.Faction == PrisonLaborUtility.GetPawnFaction(pawn));
        }

        private static bool PawnCanRepairNow(Pawn pawn, Thing t)
        {
            return PawnCanRepairEver(pawn, t) && pawn.Map.listerBuildingsRepairable.Contains(PrisonLaborUtility.GetPawnFaction(pawn), (Building)t) && t.HitPoints != t.MaxHitPoints;
        }
    }
}
