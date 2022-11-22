using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PrisonLabor.HarmonyPatches.Patches_WorkSettings
{
    [HarmonyPatch(typeof(PawnColumnWorker_AllowedArea))]
    [HarmonyPatch("DoCell")]
    class EnableAreaRestrictionsForPrisoners
    {
        static void Postfix(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn.IsPrisonerOfColony)
            {
                //Log.Message($"Pawn {pawn.LabelShort} area: {pawn.playerSettings.AreaRestriction}, is MouseBlocked: {Mouse.IsInputBlockedNow}");
                AreaAllowedGUI.DoAllowedAreaSelectors(rect, pawn);
            }
        }
    }

}