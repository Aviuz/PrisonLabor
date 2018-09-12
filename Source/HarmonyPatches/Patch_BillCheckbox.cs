using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.IO;
using Verse.Sound;

#pragma warning disable CS0252

namespace PrisonLabor.HarmonyPatches
{
    /// <summary>
    /// This patch is adding checkbox to toggle between allowed workers
    /// </summary>
    [HarmonyPatch(typeof(Dialog_BillConfig))]
    [HarmonyPatch("DoWindowContents")]
    [HarmonyPatch(new[] { typeof(Rect) })]
    internal class Patch_BillCheckbox
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            // Find >> this.bill
            OpCode[] opCodes0 =
            {
                OpCodes.Ldfld,
            };
            String[] operands0 =
            {
                "RimWorld.Bill_Production bill",
            };
            var billField = HPatcher.FindOperandAfter(opCodes0, operands0, instr);

            // Find >> Listing_Standard listing_Standard = new Listing_Standard();
            OpCode[] opCodes4 =
           {
                OpCodes.Newobj,
                OpCodes.Stloc_S,
            };
            String[] operands4 =
            {
                "Void .ctor()",
                "Verse.Listing_Standard (6)",
            };
            var listingVar = HPatcher.FindOperandAfter(opCodes4, operands4, instr);

            // Find label after >> if (listing_Standard.ButtonText(label, null))
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
                "Verse.Listing_Standard (6)",
                "System.String (7)",
                "",
                "Boolean ButtonText(System.String, System.String)",
                "System.Reflection.Emit.Label",
            };
            var label = HPatcher.FindOperandAfter(opCodes1, operands1, instr);

            // Begin rect - start of scrollable view
            int step2 = 0;
            int step4 = 0;
            // Find fragment >> listing_Standard.Begin(rect3);
            OpCode[] opCodes2 =
            {
                OpCodes.Ldloc_S,
                OpCodes.Ldloc_2,
                OpCodes.Callvirt,
            };
            String[] operands2 =
            {
                "Verse.Listing_Standard (6)",
                "",
                "Void Begin(Rect)",
            };
            // End rect - end of scrollable view
            int step3 = 0;
            // Find fragment >> listing_Standard.End();
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
                "Verse.Listing_Standard (6)",
            };

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
                if (HPatcher.IsFragment(opCodes3, operands3, ci, ref step3, "Patch_BillCheckbox1"))
                {
                    var instruction = new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("StopScrolling"));
                    instruction.labels.AddRange(ci.labels);
                    ci.labels.Clear();
                    yield return instruction;
                }
                if (HPatcher.IsFragment(opCodes2, operands2, ci, ref step4, "Patch_BillCheckbox2"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, listingVar);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("SetRect"));
                }
                yield return ci;
                if (HPatcher.IsFragment(opCodes2, operands2, ci, ref step2, "Patch_BillCheckbox3"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, listingVar);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("StartScrolling"));
                }
            }
        }

        public static void GroupExclusionButton(Listing_Standard listing, Bill bill)
        {
            string label = "no label";
            switch (BillUtility.IsFor(bill))
            {
                case GroupMode.ColonistsOnly:
                    label = "PrisonLabor_ColonistsOnlyShort".Translate();
                    break;
                case GroupMode.PrisonersOnly:
                    label = "PrisonLabor_PrisonersOnlyShort".Translate();
                    break;
                case GroupMode.ColonyOnly:
                    label = "PrisonLabor_ColonyOnlyShort".Translate();
                    break;
            }

            if (listing.ButtonText(label))
            {
                MakeModeFloatMenu(bill);
            }

            listing.Gap(12f);
        }

        private static void MakeModeFloatMenu(Bill bill)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption("PrisonLabor_ColonyOnly".Translate(), delegate
            {
                BillUtility.SetFor(bill, GroupMode.ColonyOnly);
            }));
            list.Add(new FloatMenuOption("PrisonLabor_ColonistsOnly".Translate(), delegate
            {
                BillUtility.SetFor(bill, GroupMode.ColonistsOnly);
            }));
            list.Add(new FloatMenuOption("PrisonLabor_PrisonersOnly".Translate(), delegate
            {
                BillUtility.SetFor(bill, GroupMode.PrisonersOnly);
            }));
            Find.WindowStack.Add(new FloatMenu(list));
        }

        public static Rect SetRect(Rect rect, Listing_Standard listing)
        {
            rect.height += 60 + listing.verticalSpacing + 1;
            return rect;
        }

        public static Vector2 position;
        public static void StartScrolling(Rect rect, Listing_Standard listing)
        {
            Rect viewRect = new Rect(0, 0, rect.width, rect.height + 30 + listing.verticalSpacing + 1);
            Rect outRect = new Rect(0, 0, rect.width + 16, rect.height);
            Widgets.BeginScrollView(outRect, ref position, viewRect, true);
        }

        public static void StopScrolling()
        {
            Widgets.EndScrollView();
        }
    }
}