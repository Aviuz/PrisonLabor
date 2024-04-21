using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using PrisonLabor.Core.Meta;
using PrisonLabor.Constants;
using PrisonLabor.Core;

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
        __instance.ToggleNonExclusiveInteraction(PL_DefOf.PrisonLabor_workOption, PrisonLaborPrefs.EnableWorkByDefault);
        Pawn prisoner = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        PrisonerInteractionModeDef defaultInteractionDef = DefDatabase<PrisonerInteractionModeDef>.GetNamed(PrisonLaborPrefs.DefaultInteractionMode);
        PrisonerInteractionModeDef modeToSet = prisoner.CanUsePrisonerInteraction(defaultInteractionDef) ? defaultInteractionDef : PrisonerInteractionModeDefOf.MaintainOnly;
        __instance.SetExclusiveInteraction(modeToSet);
        if (__instance.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.Convert)
        {
          __instance.ideoForConversion = Faction.OfPlayer.ideos.PrimaryIdeo;
        }
      }
    }
  }
}
