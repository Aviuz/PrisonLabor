using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_ForbidUtil
{
    [HarmonyPatch(typeof(ForbidUtility))]
    class Patch_ForbidUtility
    {
        [HarmonyPostfix]
        [HarmonyPatch("IsForbidden")]
        [HarmonyPatch(new[] { typeof(Thing), typeof(Pawn) })]
        static bool isForbidPostfix(bool __result, Thing t, Pawn pawn)
        {
            if (pawn.IsPrisonerOfColony)
            {
                DebugLogger.debug($"Prisoner {pawn.LabelShort}, forbid result: {__result} for {t}");
            }
            if (t.IsFoodForbiden(pawn))
            {
                DebugLogger.debug($"Forbid postfix food return true for {t}");
                return true;
            }

            if (pawn.IsPrisonerOfColony)
            {
                DebugLogger.debug($"Forbid postfix return { t.IsForbiddenForPrisoner(pawn)} for {t}");
                return t.IsForbiddenForPrisoner(pawn);
            }

            return __result;
        }
    }
}
