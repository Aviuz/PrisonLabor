using HarmonyLib;
using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

//Faction newFaction, Pawn recruiter = null

namespace PrisonLabor.HarmonyPatches.Patches_WorkSettings
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("SetFaction")]
    [HarmonyPatch(new[] { typeof(Faction), typeof(Pawn) })]
    class Patch_ResetWorktableWhenRecruited
    {
        private static void Prefix(Pawn __instance, Faction newFaction, Pawn recruiter)
        {          
            if (newFaction == Faction.OfPlayer)
            {
                __instance.workSettings = new Pawn_WorkSettings(__instance);
                Hediff legs = __instance.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedLegscuffs, false);
                if (legs != null)
                {
                    __instance.health.hediffSet.hediffs.Remove(legs);
                }
                Hediff hands = __instance.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedHandscuffs, false);
                if (hands != null)
                {
                    __instance.health.hediffSet.hediffs.Remove(hands);
                }
                __instance.playerSettings.AreaRestriction = null;
            }
        }
    }
}
