using HarmonyLib;
using PrisonLabor.Core.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Escaping
{
    static class Patch_EscapeTracker
    {
        //[HarmonyPatch(typeof(Pawn))]
        //[HarmonyPatch(nameof(Pawn.ExposeData))]
        //static class Patch_ExposeData
        //{
        //    static void Postfix(Pawn __instance)
        //    {
        //        var escapeTracker = EscapeTracker.Of(__instance);
        //        Scribe_Deep.Look(ref escapeTracker, "EscapeTracker", new object[] { __instance });
        //        EscapeTracker.Save(__instance, escapeTracker);
        //    }
        //}
        //
        ////TODO:[TESTING] Patches_Escaping.Patch_Tick Remove
        //[HarmonyPatch(typeof(Pawn))]
        //[HarmonyPatch(nameof(Pawn.TickRare))]
        //static class Patch_Tick
        //{
        //    static void Postfix(Pawn __instance)
        //    {
        //        if (!__instance.Dead)
        //        {
        //            EscapeTracker.Of(__instance)?.Tick();
        //        }
        //    }
        //}
    }
}
