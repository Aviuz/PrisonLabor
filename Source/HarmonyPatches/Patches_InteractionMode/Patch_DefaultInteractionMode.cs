using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PrisonLabor.Core.Meta;
using PrisonLabor.Constants;

namespace PrisonLabor.HarmonyPatches.Patches_InteractionMode
{
    [HarmonyPatch(typeof(Pawn_GuestTracker))]
    [HarmonyPatch("SetGuestStatus")]
    public class Patch_DefaultInteractionMode
    {
        private static void Postfix(Pawn_GuestTracker __instance, Faction newHost, GuestStatus guestStatus)
        {
            if (guestStatus == GuestStatus.Prisoner && newHost == Faction.OfPlayer)
            {
                __instance.interactionMode = DefDatabase<PrisonerInteractionModeDef>.GetNamed(PrisonLaborPrefs.DefaultInteractionMode);
                if(__instance.interactionMode == PL_DefOf.PrisonLabor_workAndConvertOption 
                    || __instance.interactionMode == PrisonerInteractionModeDefOf.Convert)
                {
                    __instance.ideoForConversion = Faction.OfPlayer.ideos.PrimaryIdeo;
                }               
            } 
        }
    }
}
