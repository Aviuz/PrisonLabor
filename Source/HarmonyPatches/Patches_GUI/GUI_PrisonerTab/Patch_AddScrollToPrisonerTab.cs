using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_PrisonerTab
{
    /// <summary>
    /// This patch is adding scroll bar to prisoner tab to ensure all interaction modes are visible
    /// </summary>
    [HarmonyPatch(typeof(ITab_Pawn_Visitor))]
    [HarmonyPatch("FillTab")]
    [HarmonyPatch(new Type[0])]
    internal class Patch_AddScrollToPrisonerTab
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            #region fragment>>GUI.BeginGroup(position);
            OpCode[] opCodes1 =
            {
                OpCodes.Call,
                OpCodes.Stloc_S,
                OpCodes.Ldloc_S,
                OpCodes.Call,
            };
            String[] operands1 =
            {
                "Rect ContractedBy(Rect, Single)",
                "UnityEngine.Rect (6)",
                "UnityEngine.Rect (6)",
                "Void BeginGroup(Rect)",
            };
            int step1 = 0;
            #endregion

            #region fragment>>GUI.EndGroup();
            OpCode[] opCodes2 =
            {
                OpCodes.Ldloc_S,
                OpCodes.Callvirt,
                OpCodes.Endfinally,
                OpCodes.Call,
            };
            String[] operands2 =
            {
                "System.Collections.Generic.IEnumerator`1[RimWorld.PrisonerInteractionModeDef] (9)",
                "Void Dispose()",
                "",
                "Void EndGroup()",
            };
            int step2 = 0;
            #endregion

            #region fragment>>Rect position = rect5.ContractedBy(10f);
            OpCode[] opCodes3 =
            {
                OpCodes.Ldc_R4,
                OpCodes.Call,
                OpCodes.Stloc_S,
                OpCodes.Ldloc_S,
            };
            String[] operands3 =
            {
                "10",
                "Rect ContractedBy(Rect, Single)",
                "UnityEngine.Rect (6)",
                "UnityEngine.Rect (6)",
            };
            int step3 = 0;
            var rect = HPatcher.FindOperandAfter(opCodes3, operands3, instr);
            #endregion

            foreach (var ci in instr)
            {
                // end scroll
                if (HPatcher.IsFragment(opCodes2, operands2, ci, ref step2, "AddScrollToPrisonerTab2"))
                {
                    var instruction = new CodeInstruction(OpCodes.Call, typeof(Patch_AddScrollToPrisonerTab).GetMethod(nameof(StopScrolling)));
                    instruction.labels.AddRange(ci.labels);
                    ci.labels.Clear();
                    yield return instruction;
                }

                // resize
                if (HPatcher.IsFragment(opCodes3, operands3, ci, ref step3, "AddScrollToPrisonerTab3"))
                {

                }

                yield return ci;

                // begin scroll
                if (HPatcher.IsFragment(opCodes1, operands1, ci, ref step1, "AddScrollToPrisonerTab1"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, rect);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_AddScrollToPrisonerTab).GetMethod(nameof(StartScrolling)));
                    yield return new CodeInstruction(OpCodes.Stloc_S, rect);
                }
            }
        }

        public static Vector2 position;
        public static Rect StartScrolling(Rect rect)
        {
            Rect viewRect = new Rect(0, 0, rect.width - 16, rect.height + 56);
            Rect outRect = new Rect(0, 0, rect.width, rect.height);
            Widgets.BeginScrollView(outRect, ref position, viewRect, true);
            return viewRect;
        }

        public static void StopScrolling()
        {
            Widgets.EndScrollView();
        }
    }
}
