using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using PrisonLabor.Core;

namespace PrisonLabor.HarmonyPatches.Patches_InteractionMode
{
    [HarmonyPatch()]
    class HemogenFarmInteractionPatch
    {

        [HarmonyPatch(typeof(Pawn_GuestTracker), "GuestTrackerTick")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> GuestTrackerTickTranspiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
        {
            OpCode[] opCodesToFind =
            {
                OpCodes.Ldfld,
                OpCodes.Ldsfld,
                OpCodes.Bne_Un
            };
            string[] operandsToFind =
            {
                "RimWorld.PrisonerInteractionModeDef interactionMode",
                "RimWorld.PrisonerInteractionModeDef HemogenFarm",
                "System.Reflection.Emit.Label"
            };
            var label = (Label)HPatcher.FindOperandAfter(opCodesToFind, operandsToFind, instructions);
            CodeInstruction[] replacment =
            {
               new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.interactionMode))),
               new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod("HemogenFarmInteractionMode")),
               new CodeInstruction(OpCodes.Brfalse_S, label)
            };
            return HPatcher.ReplaceFragment(opCodesToFind, operandsToFind, instructions, replacment, nameof(Pawn_GuestTracker) + ": GuestTrackerTick patch");
        }

        [HarmonyPatch(typeof(Alert_AwaitingMedicalOperation), "get_AwaitingMedicalOperation")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Alert_AwaitingMedicalOperationTranspiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
        {
            HPatcher.CreateDebugFileOnDesktop("Alert_AwaitingMedicalOperation", instructions);
            OpCode[] opCodesToFind =
{
                OpCodes.Ldfld,
                OpCodes.Ldsfld,
                OpCodes.Beq_S
            };
            string[] operandsToFind =
            {
                "RimWorld.PrisonerInteractionModeDef interactionMode",
                "RimWorld.PrisonerInteractionModeDef HemogenFarm",
                "System.Reflection.Emit.Label"
            };
            var label = (Label)HPatcher.FindOperandAfter(opCodesToFind, operandsToFind, instructions);
            CodeInstruction[] replacment =
            {
               new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.interactionMode))),
               new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod("HemogenFarmInteractionMode")),
               new CodeInstruction(OpCodes.Brtrue_S, label)
            };

            return HPatcher.ReplaceFragment(opCodesToFind, operandsToFind, instructions, replacment, nameof(Alert_AwaitingMedicalOperation) + ": get_AwaitingMedicalOperation patch"); ;
        }

    }
}
