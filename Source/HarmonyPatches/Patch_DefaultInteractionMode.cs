using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn_GuestTracker))]
    [HarmonyPatch("SetGuestStatus")]
    public class Patch_DefaultInteractionMode
    {
        private static void Postfix(Pawn_GuestTracker __instance, Faction newHost, bool prisoner)
        {
            if (prisoner == true)
            {
                __instance.interactionMode = DefDatabase<PrisonerInteractionModeDef>.GetNamed(PrisonLaborPrefs.DefaultInteractionMode);
            }
        }
    }
}
