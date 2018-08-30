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

            // Find label after >> if (listing_Standard.ButtonText(label, null))
            OpCode[] opCodes1 =
            {
                OpCodes.Ldstr,
                OpCodes.Call,
                OpCodes.Ldnull,
                OpCodes.Callvirt,
                OpCodes.Brfalse
            };
            String[] operands1 =
            {
                "NotSuspended",
                "System.String Translate(System.String)",
                "",
                "Boolean ButtonText(System.String, System.String)",
                "System.Reflection.Emit.Label",
            };
            var label = HPatcher.FindOperandAfter(opCodes1, operands1, instr);

            // TODO old code
            /*
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
            */

            // Find listing standard for buttons on left
            OpCode[] opCodes4 =
           {
                OpCodes.Call,
                OpCodes.Br,
                OpCodes.Ldloc_S,
            };
            String[] operands4 =
            {
                "Void PlayOneShotOnCamera(Verse.SoundDef, Verse.Map)",
                "System.Reflection.Emit.Label",
                "Verse.Listing_Standard (30)",
            };
            var listing = HPatcher.FindOperandAfter(opCodes4, operands4, instr);

            foreach (var ci in instr)
            {
                if (ci.labels.Contains((Label)label))
                {
                    var injectedInstruction = new CodeInstruction(OpCodes.Ldloc_S, listing);
                    foreach(var item in ci.labels)
                        injectedInstruction.labels.Add(item);
                    yield return injectedInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, billField);
                    yield return new CodeInstruction(OpCodes.Call,
                        typeof(Patch_BillCheckbox).GetMethod("GroupExclusionButton"));
                    ci.labels.Clear();
                }
                //TODO old code
                /*
                if (HPatcher.IsFragment(opCodes3, operands3, ci, ref step3, "Patch_BillCheckbox1"))
                {
                    var instruction = new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("StopScrolling"));
                    instruction.labels.AddRange(ci.labels);
                    ci.labels.Clear();
                    yield return instruction;
                }
                if (HPatcher.IsFragment(opCodes2, operands2, ci, ref step4, "Patch_BillCheckbox2"))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("SetRect"));
                }
                */
                yield return ci;
                /*
                if (HPatcher.IsFragment(opCodes2, operands2, ci, ref step2, "Patch_BillCheckbox3"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod("StartScrolling"));
                }
                */
            }
        }

        public static void GroupExclusionButton(Listing_Standard listing, Bill bill)
        {
            if (BillUtility.IsFor(bill) == GroupMode.ColonistsOnly)
            {
                if (listing.ButtonText("PrisonLabor_ColonistsOnly".Translate()))
                {
                    BillUtility.SetFor(bill, GroupMode.PrisonersOnly);
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
            }
            else if (BillUtility.IsFor(bill) == GroupMode.PrisonersOnly)
            {
                if (listing.ButtonText("PrisonLabor_PrisonersOnly".Translate()))
                {
                    BillUtility.SetFor(bill, GroupMode.ColonyOnly);
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
            }
            else
            {
                if (listing.ButtonText("PrisonLabor_ColonyOnly".Translate()))
                {
                    BillUtility.SetFor(bill, GroupMode.ColonistsOnly);
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
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