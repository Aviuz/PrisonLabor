using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PrisonLabor.Core.LaborArea;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    //[HarmonyPatch(typeof(Building_Door))]
    //[HarmonyPatch(nameof(Building_Door.PawnCanOpen))]
    //public static class Patch_DoorAccess
    //{
    //    public static bool Prefix(out Pawn __state, Pawn p) { __state = p; return true; }

    //    public static void Postfix(ref bool __result, Building_Door __instance, Pawn __state)
    //    {
    //        if (!__state.IsPrisoner)
    //            return;

    //        var bed = __state.CurrentBed();
    //        if (bed == null)
    //        {
    //            __result = false;
    //            return;
    //        }
    //        var cell = bed.GetRoom();
    //        if (cell == null)
    //        {
    //            __result = false;
    //            return;
    //        }

    //        if (__state.timetable.CurrentAssignment != TimeAssignmentDefOf.Work && __state.IsPrisonerInPrisonCell())
    //        {
    //            __result = false;
    //            return;
    //        }

    //        var doorPos = __instance.Position;
    //        var laborArea = PrisonLabor.Core.Trackers.InspirationTracker.map.areaManager.Get<Area_Labor>();

    //        if (!new List<IntVec3>(laborArea.ActiveCells).Contains(doorPos))
    //        {
    //            __result = false;
    //            return;
    //        }

    //        __result = true;
    //    }
    //}
}
