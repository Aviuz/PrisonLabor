using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_InteractionMode
{
    [HarmonyPatch(typeof(Pawn_GuestTracker))]
    [HarmonyPatch("SetGuestStatus")]
    public class Patch_RemoveHediffsAfterStatusChange
    {
        private static void Postfix(Pawn_GuestTracker __instance, Faction newHost, GuestStatus guestStatus)
        {
            if (guestStatus != GuestStatus.Prisoner)
            {
                Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
                CleanPrisonersStatus.CleanHediffs(pawn);
            }

            if (guestStatus == GuestStatus.Prisoner)
            {
                Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
                if (pawn.drugs == null)
                {
                    pawn.drugs = new Pawn_DrugPolicyTracker(pawn);
                }
            }
        }
    }
}
