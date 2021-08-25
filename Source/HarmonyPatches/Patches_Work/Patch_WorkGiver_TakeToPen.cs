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
    [HarmonyPatch(typeof(WorkGiver_TakeToPen), "PotentialWorkThingsGlobal")]
    class Patch_WorkGiver_TakeToPen
    {
        static bool Prefix(ref IEnumerable<Thing> __result, Pawn pawn)
        {
            if (pawn != null && pawn.IsPrisonerOfColony)
            {
                __result = pawn.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
                return false;
            }

            return true;
        }
    }
}
