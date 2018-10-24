using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPatch(new[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>) })]
    public static class Trigger_AddHumanlikeOrders
    {
        public static void Prefix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            ArrestUtility.AddArrestOrder(clickPos, pawn, opts);
        }
    }
}
