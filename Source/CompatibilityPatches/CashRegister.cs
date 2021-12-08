using CashRegister.Shifts;
using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.CompatibilityPatches
{
    static class CashRegister
    {
        private static ModSearcher modSeeker;
        internal static void Init()
        {
            ModSearcher modSeeker = new ModSearcher("Cash Register");
            if (modSeeker.LookForMod())
            {
                Patch();
            }
        }

        private static void Patch()
        {
            try
            {
                MethodBase methodBase = GetTargetMethod();
                if (methodBase != null)
                {
                    var harmony = new Harmony("Harmony_PrisonLabor_CashRegister");
                    harmony.Patch(methodBase, postfix: new HarmonyMethod(typeof(CashRegister).GetMethod("PostFixCandidates")));
                    Log.Message("Prison Labor: Cash Register mod patched");
                }
            }
            catch (Exception e)
            {
                Log.Error($"PrisonLaborException: failed to proceed Cash Register mod patches: {e.ToString()}");
            }

        }

        public static IEnumerable<Pawn> PostFixCandidates(IEnumerable<Pawn> __result, CompAssignableToPawn __instance)
        {
            foreach(Pawn pawn in __result)
            {
                yield return pawn;
            }

            if (__instance.parent.Spawned && __instance is CompAssignableToPawn_Shifts)
            {
                foreach(Pawn pawn in __instance.parent.Map.mapPawns.PrisonersOfColonySpawned)
                {
                    yield return pawn;
                }
            }          
        }

        private static MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(CompAssignableToPawn), "get_AssigningCandidates");
        }
    }

}
