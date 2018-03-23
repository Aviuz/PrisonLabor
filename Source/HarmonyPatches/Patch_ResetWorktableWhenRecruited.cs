using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

//Faction newFaction, Pawn recruiter = null

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("SetFaction")]
    [HarmonyPatch(new[] { typeof(Faction), typeof(Pawn) })]
    class Patch_ResetWorktableWhenRecruited
    {
        private static void Prefix(Pawn __instance, Faction newFaction, Pawn recruiter)
        {
            if(__instance.IsPrisonerOfColony && newFaction == Faction.OfPlayer)
            {
                __instance.workSettings = null;
            }
        }
    }
}
