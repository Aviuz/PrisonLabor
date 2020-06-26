using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches
{


    [HarmonyPatch(typeof(InteractionWorker))]
    [HarmonyPatch(nameof(InteractionWorker.Interacted))]
    public static class InspirationTracker
    {
        public static void Postfix(Pawn initiator, Pawn recipient)
        {
            // Social interactions between prisoners and colonists.
            if (initiator.IsPrisonerOfColony && recipient.IsColonist)
            {
                PrisonLabor.Core.Trackers.InspirationTracker.PrisonerToWarden(initiator, recipient);
            }
            else if (recipient.IsPrisonerOfColony && initiator.IsColonist)
            {
                PrisonLabor.Core.Trackers.InspirationTracker.PrisonerToWarden(recipient, initiator);
            }
            else if (recipient.IsPrisonerOfColony && initiator.IsPrisonerOfColony)
            {
                PrisonLabor.Core.Trackers.InspirationTracker.PrisonerToPrisoner(initiator, recipient);
            }
        }
    }

    [HarmonyPatch(typeof(Map))]
    [HarmonyPatch("FinalizeInit")]
    [HarmonyPatch(new Type[] { })]
    public static class MapLoaded
    {
        public static void Postfix(Map __instance)
        {
            PrisonLabor.Core.Trackers.InspirationTracker.defOfTreatedBadly = DefDatabase<ThoughtDef>.GetNamed("PrisonLabor_TreatedBadly");
            PrisonLabor.Core.Trackers.InspirationTracker.defOfTreatedWell = DefDatabase<ThoughtDef>.GetNamed("PrisonLabor_VeryGoodTreatment");
            PrisonLabor.Core.Trackers.InspirationTracker.defOfLowMotivation = DefDatabase<ThoughtDef>.GetNamed("PrisonLabor_LowMotivation");
            PrisonLabor.Core.Trackers.InspirationTracker.defOfHorrer = DefDatabase<ThoughtDef>.GetNamed("PrisonLabor_Horrer");

            PrisonLabor.Core.Trackers.InspirationTracker.map = __instance;
        }
    }

    [HarmonyPatch(typeof(Map))]
    [HarmonyPatch(nameof(Map.MapPreTick))]
    public static class MapTick
    {
        public static void Postfix() { PrisonLabor.Core.Trackers.InspirationTracker.Calculate(); }
    }
}
