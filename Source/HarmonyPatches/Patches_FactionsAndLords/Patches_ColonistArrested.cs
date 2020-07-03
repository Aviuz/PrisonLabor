using System;
using HarmonyLib;
using PrisonLabor.Core.Components;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace PrisonLabor.HarmonyPatches.Patches_FactionsAndLords
{
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch(nameof(Faction.Notify_MemberCaptured))]
    public static class Patches_ColonistArrested
    {
        public static void Postfix(Pawn member)
        {
            lock (Tracked.LOCK_WARDEN)
                Tracked.CleanUp();
#if TRACE
            Log.Message("Pawn Arrest Attempted");
#endif
        }
    }
}