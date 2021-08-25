using HarmonyLib;
using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_InteractionMode
{
    [HarmonyPatch(typeof(ITab_Pawn_Visitor), "FillTab")]
    class Patch_VisitorTab_TabDraw
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
        {
            OpCode[] opCodesToFind =
            {
                OpCodes.Call,
                OpCodes.Ldfld,
                OpCodes.Ldfld,
                OpCodes.Ldsfld,
                OpCodes.Bne_Un
            };
            string[] operandsToFind =
            {
                "Verse.Pawn get_SelPawn()",
                "RimWorld.Pawn_GuestTracker guest",
                "RimWorld.PrisonerInteractionModeDef interactionMode",
                "RimWorld.PrisonerInteractionModeDef Convert",
                "System.Reflection.Emit.Label"
            };

            OpCode[] opCodesToReplace =
            {
                OpCodes.Call,
                OpCodes.Ldfld,
                OpCodes.Ldfld,
                OpCodes.Ldsfld,
                OpCodes.Bne_Un
            };
            string[] operandsToReplace =
            {
                "Verse.Pawn get_SelPawn()",
                "RimWorld.Pawn_GuestTracker guest",
                "RimWorld.PrisonerInteractionModeDef interactionMode",
                "RimWorld.PrisonerInteractionModeDef Convert",
                "System.Reflection.Emit.Label"
            };

            var label = (Label)HPatcher.FindOperandAfter(opCodesToFind, operandsToFind, instructions);
            CodeInstruction[] replacment =
            {
               new CodeInstruction(OpCodes.Call, typeof(Patch_VisitorTab_TabDraw).GetMethod(nameof(ShouldDisplayConvertIco))),
               new CodeInstruction(OpCodes.Brfalse, label)
            };

            return HPatcher.ReplaceFragment(opCodesToReplace, operandsToReplace, instructions, replacment, nameof(ITab_Pawn_Visitor) + ": patch display ideo ico");
        }

        public static bool ShouldDisplayConvertIco(ITab_Pawn_Visitor tab)
        {
            Pawn p = Traverse.Create(tab).Property("SelPawn").GetValue<Pawn>();
            return p.guest.interactionMode == PrisonerInteractionModeDefOf.Convert || p.guest.interactionMode == PL_DefOf.PrisonLabor_workAndConvertOption;
        }
    }
}
