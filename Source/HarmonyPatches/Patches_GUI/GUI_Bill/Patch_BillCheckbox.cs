using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.IO;
using Verse.Sound;
using PrisonLabor.Core.BillAssignation;

#pragma warning disable CS0252

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_Bill
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

            // Find label after >> if (listing_Standard.ButtonText(label, null))
            OpCode[] opCodes1 =
            {
                OpCodes.Ldstr,
                OpCodes.Call,
                OpCodes.Call,
                OpCodes.Ldnull,
                OpCodes.Callvirt,
                OpCodes.Brfalse_S
            };
            String[] operands1 =
            {
                "NotSuspended",
                "Verse.TaggedString Translate(System.String)",
                "System.String op_Implicit(Verse.TaggedString)",
                "",
                "Boolean ButtonText(System.String, System.String)",
                "System.Reflection.Emit.Label",
            };
            var label = HPatcher.FindOperandAfter(opCodes1, operands1, instr);

            // Find listing standard for buttons on left
            OpCode[] opCodes4 =
           {
                OpCodes.Call,
                OpCodes.Br_S,
                OpCodes.Ldloc_S,
            };
            String[] operands4 =
            {
                "Void PlayOneShotOnCamera(Verse.SoundDef, Verse.Map)",
                "System.Reflection.Emit.Label",
                "Verse.Listing_Standard (31)",
            };
            var listing = HPatcher.FindOperandAfter(opCodes4, operands4, instr);

            foreach (var ci in instr)
            {
                if (ci.labels.Contains((Label)label))
                {
                    var injectedInstruction = new CodeInstruction(OpCodes.Ldloc_S, listing);
                    foreach (var item in ci.labels)
                        injectedInstruction.labels.Add(item);
                    yield return injectedInstruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, billField);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Patch_BillCheckbox).GetMethod(nameof(GroupExclusionButton)));
                    ci.labels.Clear();
                }

                yield return ci;
            }
        }

        public static void GroupExclusionButton(Listing_Standard listing, Bill bill)
        {
            string label;
            switch (BillAssignationUtility.IsFor(bill))
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
                default:
                    label = "no label";
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
                BillAssignationUtility.SetFor(bill, GroupMode.ColonyOnly);
            }));
            list.Add(new FloatMenuOption("PrisonLabor_ColonistsOnly".Translate(), delegate
            {
                BillAssignationUtility.SetFor(bill, GroupMode.ColonistsOnly);
            }));
            list.Add(new FloatMenuOption("PrisonLabor_PrisonersOnly".Translate(), delegate
            {
                BillAssignationUtility.SetFor(bill, GroupMode.PrisonersOnly);
            }));
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}
