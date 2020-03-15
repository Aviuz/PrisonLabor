using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_PermissionFix
{
    [HarmonyPatch(typeof(Pawn_CarryTracker))]
    [HarmonyPatch("TryDropCarriedThing")]
    [HarmonyPatch(new[] {typeof(IntVec3), typeof(int), typeof(ThingPlaceMode), typeof(Thing), typeof(Action<Thing, int>) }, 
        new ArgumentType[] {ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, })]
    internal class ForibiddenDropPatch2
    {
        public static void Postfix(Pawn_CarryTracker __instance, int count, IntVec3 dropLoc, ThingPlaceMode mode,
            Thing resultingThing, Action<Thing, int> placedAction = null)
        {
            ForibiddenDropPatch.Postfix(__instance, dropLoc, mode, resultingThing, placedAction);
        }
    }
}