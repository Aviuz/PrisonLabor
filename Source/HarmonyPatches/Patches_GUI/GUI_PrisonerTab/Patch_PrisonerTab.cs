using HarmonyLib;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_PrisonerTab
{
    /// <summary>
    /// This patch is adding:
    ///     1. string in dev mode indicating percentage of being unwatched before escaping
    ///     2. Recruit option
    /// </summary>
    [HarmonyPatch(typeof(ITab_Pawn_Visitor), "FillTab")]
    public static class Patch_PrisonerTab
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            // "listing_Standard.End()" fragment
            OpCode[] opCodes =
            {
                OpCodes.Ldloc_1,
                OpCodes.Callvirt,
            };
            string[] operands =
            {
                "",
                "Void End()",
            };
            int step2 = 0;

            foreach (var ci in instr)
            {
                if (HPatcher.IsFragment(opCodes, operands, ci, ref step2, "Patch_PrisonerTab listing_standard.End()"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_PrisonerTab).GetMethod(nameof(AddRecruitButton)));

                    // Append Dev lines (When dev mode is on)
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_PrisonerTab).GetMethod(nameof(AppendDevLines)));
                }

                yield return ci;

            }
        }

        public static void AppendDevLines(Listing_Standard listingStandard)
        {
            if (Prefs.DevMode)
            {
                var pawn = Find.Selector.SingleSelectedThing as Pawn;
                var escapeTracker = EscapeTracker.Of(pawn);
                if (escapeTracker != null)
                    listingStandard.Label(
                        "Dev: Ready to escape: " +
                        (escapeTracker.ReadyToEscape ? "ready" : escapeTracker.ReadyToRunPercentage + "%") +
                        $" (Cap:{escapeTracker.EscapeLevel})", -1f);
            }
        }

        public static void AddRecruitButton(Listing_Standard listingStandard)
        {
            var pawn = Find.Selector.SingleSelectedThing as Pawn;
            var need = pawn.needs.TryGetNeed<Need_Treatment>();
            if (need != null && need.ResocializationReady)
            {
                if (listingStandard.ButtonTextLabeled("PrisonLabor_RecruitButtonDesc".Translate(), "PrisonLabor_RecruitButtonLabel".Translate()))
                {
                    pawn.guest.SetGuestStatus(null);
                    pawn.SetFaction(Faction.OfPlayer);
                }
            }
        }
    }
}
