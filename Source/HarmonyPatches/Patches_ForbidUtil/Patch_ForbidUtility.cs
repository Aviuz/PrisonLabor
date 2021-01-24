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
            if (t.IsFoodForbiden(pawn))
            {
                return true;
            }

            if (pawn.IsPrisonerOfColony)
            {
                return t.IsForbiddenForPrisoner(pawn);
            }

            return __result;
        }
    }
}
