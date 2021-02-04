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
    [HarmonyPatch(typeof(WorkGiver_RescueDowned), "ShouldSkip")]

    class Patch_WorkGiver_RescueDowned
    {
        static bool Prefix(ref bool __result, Pawn pawn, bool forced)
        {
            if(pawn != null && pawn.IsPrisonerOfColony)
            {
                List<Pawn> list = pawn.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Downed && !list[i].InBed())
                    {
                       __result = false;
                        return false;
                    }
                }
                __result = true;
                return false;
            }

            return true;
        }
    }
}
