using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{

    [HarmonyPatch(typeof(WorkGiver_CarryToBuilding), "FindBuildingFor")]
    public class Patch_CarryToBuilding
    {
        static Building Postfix(Building __result, Pawn pawn, Pawn traveller, bool forced)
        {
            if (traveller.IsPrisonerOfColony && pawn.IsPrisonerOfColony && traveller == pawn)
            {
                return null;
            }
            return __result;
        }
    }
}
