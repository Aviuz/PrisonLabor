using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.IO;

#pragma warning disable CS0252

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Dialog_BillConfig))]
    [HarmonyPatch("DoWindowContents")]
    [HarmonyPatch(new[] { typeof(Rect) })]
    internal class Patch_BillCheckbox
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            // Find operands
            OpCode[] opCodes0 =
            {
                OpCodes.Ldfld,
            };
            String[] operands0 =
            {
                "RimWorld.Bill_Production bill",
            };
            var billField = HPatcher.FindOperandAfter(opCodes0, operands0, instr);
            OpCode[] opCodes1 =
            {
                OpCodes.Ldloc_S,
                OpCodes.Ldloc_S,
                OpCodes.Ldnull,
                OpCodes.Callvirt,
                OpCodes.Brfalse
            };
            String[] operands1 =
            {
                "Verse.Listing_Standard (4)",
                "System.String (5)",
                "",
                "Boolean ButtonText(System.String, System.String)",
                "System.Reflection.Emit.Label",
            };
            var label = HPatcher.FindOperandAfter(opCodes1, operands1, instr);

            // Begin rect - start of scrollable view
            int step2 = 0;
            int step4 = 0;
            OpCode[] opCodes2 =
            {
                OpCodes.Ldloc_S,
                OpCodes.Ldloc_2,
                OpCodes.Callvirt,
            };
            String[] operands2 =
            {
                "Verse.Listing_Standard (4)",
                "",
                "Void Begin(Rect)",
            };
            // End rect - end of scrollable view
            int step3 = 0;
            OpCode[] opCodes3 =
            {
                OpCodes.Call,
                OpCodes.Stfld,
                OpCodes.Ldloc_S,
            };
            String[] operands3 =
            {
                "Int32 RoundToInt(Single)",
                "System.Int32 unpauseWhenYouHave",
                "Verse.Listing_Standard (4)",
            };


            // Transpiller
            if (billField != null && label != null)
            {
                foreach (var ci in instr)
                {
                    if (ci.labels.Contains((Label)label))
                    {
                        var injectedInstruction = new CodeInstruction(OpCodes.Ldloc_S, ci.operand);
                        injectedInstruction.labels.Add((Label)label);
                        yield return injectedInstruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, billField);
                        yield return new CodeInstruction(OpCodes.Call,
                            typeof(Patch_BillCheckbox).GetMethod("GroupExclusionButton"));
                        ci.labels.Remove((Label)label);
                    }
                    if(HPatcher.IsFragment(opCodes3, operands3, ci, ref step3))
                    {
                        var instruction = new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("StopScrolling"));
                        instruction.labels.AddRange(ci.labels);
                        ci.labels.Clear();
                        yield return instruction;
                    }
                    if (HPatcher.IsFragment(opCodes2, operands2, ci, ref step4))
                    {
                        yield return new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("SetRect"));
                    }
                    yield return ci;
                    if(HPatcher.IsFragment(opCodes2, operands2, ci, ref step2))
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_2);
                        yield return new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("StartScrolling"));
                    }
                }
            }
            else
            {
                throw new Exception($"Failed to get operands for harmony patch Patch_BillPrevention: billField: {billField != null}, label: {label != null}");
            }

        }

        public static void GroupExclusionButton(Listing_Standard listing, Bill bill)
        {
            if (BillUtility.IsFor(bill) == GroupMode.ColonistsOnly)
            {
                if (listing.ButtonText("PrisonLabor_ColonistsOnly".Translate()))
                    BillUtility.SetFor(bill, GroupMode.PrisonersOnly);
            }
            else if (BillUtility.IsFor(bill) == GroupMode.PrisonersOnly)
            {
                if (listing.ButtonText("PrisonLabor_PrisonersOnly".Translate()))
                    BillUtility.SetFor(bill, GroupMode.ColonyOnly);
            }
            else
            {
                if (listing.ButtonText("PrisonLabor_ColonyOnly".Translate()))
                    BillUtility.SetFor(bill, GroupMode.ColonistsOnly);
            }
            listing.Gap(12f);
        }

        public static Rect SetRect(Rect rect)
        {
            rect.height += 32;
            rect.width -= 16;
            return rect;
        }

        public static Vector2 position;
        public static void StartScrolling(Rect rect)
        {
            Rect viewRect = new Rect(0, 0, rect.width - 16, rect.height + 32);
            Rect outRect = new Rect(0, 0, rect.width, rect.height);
            Widgets.BeginScrollView(outRect, ref position, viewRect, true);
        }

        public static void StopScrolling()
        {
            Widgets.EndScrollView();
        }
    }
}