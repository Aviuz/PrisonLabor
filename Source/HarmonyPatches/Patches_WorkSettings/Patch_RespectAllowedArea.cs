using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_WorkSettings
{
    [HarmonyPatch(typeof(Pawn_PlayerSettings))]
    [HarmonyPatch("get_RespectsAllowedArea")]
    class Patch_RespectAllowedArea
    {
        static bool Postfix(bool __result, Pawn_PlayerSettings __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (!__result && pawn != null && pawn.IsPrisonerOfColony )
            {
                return true;
            }
            return __result;
        }
    }
}
