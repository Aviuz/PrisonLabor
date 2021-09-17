using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_TakeToBed
{

    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPatch(new[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>) })]
    public class Patch_AddTakeToBed
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            ArrestUtility.AddTakeToBedOrder(clickPos, pawn, opts);
        }
    }


}
