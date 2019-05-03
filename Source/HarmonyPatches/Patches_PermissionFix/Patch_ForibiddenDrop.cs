using System;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_PermissionFix
{
    public class ForibiddenDropPatch
    {
        public static void Postfix(Pawn_CarryTracker __instance, IntVec3 dropLoc, ThingPlaceMode mode,
            Thing resultingThing, Action<Thing, int> placedAction = null)
        {
            if (resultingThing.IsForbidden(Faction.OfPlayer) && __instance.pawn.IsPrisonerOfColony)
                resultingThing.SetForbidden(false);
        }

        public static void Postfix2(Pawn_CarryTracker __instance, int count, IntVec3 dropLoc, ThingPlaceMode mode,
            Thing resultingThing, Action<Thing, int> placedAction = null)
        {
            Postfix(__instance, dropLoc, mode, resultingThing, placedAction);
        }
    }
}