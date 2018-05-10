using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches
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
            // "if (Prefs.DevMode)" fragment
            OpCode[] opCodes1 =
            {
                OpCodes.Call,
                OpCodes.Brfalse,
            };
            string[] operands1 =
            {
                "Boolean get_DevMode()",
                "System.Reflection.Emit.Label",
            };
            int step1 = 0;

            // "listing_Standard.End()" fragment
            OpCode[] opCodes2 =
            {
                OpCodes.Ldloc_3,
                OpCodes.Callvirt,
            };
            string[] operands2 =
            {
                "",
                "Void End()",
            };
            int step2 = 0;

            foreach (var ci in instr)
            {
                if (HPatcher.IsFragment(opCodes2, operands2, ci, ref step2, "Patch_PrisonerTab listing_standard.End()"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_PrisonerTab).GetMethod("AddRecruitButton"));
                }

                yield return ci;

                if (HPatcher.IsFragment(opCodes1, operands1, ci, ref step1, "Patch_PrisonerTab IfDevMode"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_PrisonerTab).GetMethod("AppendDevLines"));
                }

            }
        }

        public static void AppendDevLines(Listing_Standard listingStandard)
        {
            var pawn = Find.Selector.SingleSelectedThing as Pawn;
            var need = pawn.needs.TryGetNeed<Need_Motivation>();
            if (need != null)
                listingStandard.Label("Dev: Ready to escape: " + (need.ReadyToRun ? "ready" : need.ReadyToRunPercentage + "%"), -1f);
        }

        public static void AddRecruitButton(Listing_Standard listingStandard)
        {
            var pawn = Find.Selector.SingleSelectedThing as Pawn;
            var need = pawn.needs.TryGetNeed<Need_Treatment>();
            if (need != null && need.ResocializationReady)
            {
                if (listingStandard.ButtonTextLabeled("PrisonLabor_RecruitButtonDesc".Translate(), "PrisonLabor_RecruitButtonLabel".Translate()))
                {
                    pawn.SetFaction(Faction.OfPlayer);
                }
            }
        }
    }
}
