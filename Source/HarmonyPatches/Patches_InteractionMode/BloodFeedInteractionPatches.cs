using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_InteractionMode
{
    [HarmonyPatch()]
    class BloodFeedInteractionPatches
    {
        [HarmonyPatch(typeof(TargetingParameters), nameof(TargetingParameters.ForBloodfeeding))]
        [HarmonyPostfix]
        public static TargetingParameters TargetingParametersPostFix(TargetingParameters __result, Pawn pawn)
        {
            __result.validator = delegate (TargetInfo targ)
            {
                if (!(targ.Thing is Pawn pawn2))
                {
                    return false;
                }
                return (pawn2.IsPrisonerOfColony && pawn2.guest.PrisonerIsSecure && PrisonLaborUtility.BloodFeedInteractionMode(pawn2.guest.interactionMode));
            };

            return __result;
        }

        [HarmonyPatch(typeof(JobGiver_GetHemogen), "CanFeedOnPrisoner")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> GuestTrackerTickTranspiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
        {
            OpCode[] opCodesToFind =
            {
                OpCodes.Ldfld,
                OpCodes.Ldsfld,
                OpCodes.Bne_Un_S
            };
            string[] operandsToFind =
            {
                "RimWorld.PrisonerInteractionModeDef interactionMode",
                "RimWorld.PrisonerInteractionModeDef Bloodfeed",
                "System.Reflection.Emit.Label"
            };
            var label = (Label)HPatcher.FindOperandAfter(opCodesToFind, operandsToFind, instructions);
            CodeInstruction[] replacment =
            {
               new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.interactionMode))),
               new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod("BloodFeedInteractionMode")),
               new CodeInstruction(OpCodes.Brfalse_S, label)
            };
            return HPatcher.ReplaceFragment(opCodesToFind, operandsToFind, instructions, replacment, nameof(JobGiver_GetHemogen) + ": CanFeedOnPrisoner patch");
        }
    }
}
