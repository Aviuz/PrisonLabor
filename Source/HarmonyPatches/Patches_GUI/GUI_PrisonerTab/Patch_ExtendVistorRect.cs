using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_PrisonerTab
{
    [HarmonyPatch(typeof(ITab_Pawn_Visitor))]
    [HarmonyPatch("FillTab")]
    class Patch_ExtendVistorRect
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {

            foreach (var ci in instr)
            {
                if (ci.operand is float && (float)ci.operand == 200f)
                    ci.operand = 30f * DefDatabase<PrisonerInteractionModeDef>.DefCount + 10;
                yield return ci;
            }
        }
    }
}
