using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using CashRegister.Shifts;

namespace CashRegistryCompatibility.HarmonyPatches
{
    [HarmonyPatch]
    class PrisonerAssignPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(CompAssignableToPawn), "get_AssigningCandidates");
        }
        static IEnumerable<Pawn> Postfix(IEnumerable<Pawn> __result, CompAssignableToPawn __instance)
        {
            foreach (Pawn pawn in __result)
            {
                yield return pawn;
            }

            if (__instance.parent.Spawned && __instance is CompAssignableToPawn_Shifts)
            {
                foreach (Pawn pawn in __instance.parent.Map.mapPawns.PrisonersOfColonySpawned)
                {
                    yield return pawn;
                }
            }
        }
    }
}
