using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.BillAssignation;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_Bill
{
    [HarmonyPatch(typeof(Dialog_BillConfig))]
    class Patch_RestrictBillToPrisoner
    {

        /// <summary>
        /// Replace button label to allow select more element
        /// Oryginal:
        /// Widgets.Dropdown<Bill_Production, Pawn>(buttonLabel: (bill.PawnRestriction != null) ? bill.PawnRestriction.LabelShortCap : ((!ModsConfig.IdeologyActive || !bill.SlavesOnly) ? ((string)"AnyWorker".Translate()) : ((string)"AnySlave".Translate()))
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        [HarmonyTranspiler]
        [HarmonyPatch("DoWindowContents")]
        [HarmonyPatch(new[] { typeof(Rect) })]
        static IEnumerable<CodeInstruction> Transpiler_DoWindowContent(IEnumerable<CodeInstruction> instructions)
        {
            /*
                call | Boolean get_IdeologyActive() | Label 39
                brfalse.s | Label 41 | no labels
                ldarg.0 |  | no labels
                ldfld | RimWorld.Bill_Production bill | no labels
                callvirt | Boolean get_SlavesOnly() | no labels
                brfalse.s | Label 42 | no labels
                ldstr | AnySlave | no labels
                call | Verse.TaggedString Translate(System.String) | no labels
                call | System.String op_Implicit(Verse.TaggedString) | no labels
                stloc.s | System.String (25) | no labels
                br.s | Label 43 | no labels
                ldstr | AnyWorker | Label 41Label 42
                call | Verse.TaggedString Translate(System.String) | no labels
                call | System.String op_Implicit(Verse.TaggedString) | no labels
                stloc.s | System.String (25) | no labels
             */
            CodeInstruction[] replacement =
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, typeof(Patch_RestrictBillToPrisoner).GetMethod(nameof(GetLabel))),
                new CodeInstruction(OpCodes.Stloc_S, 25)
            };
            // return HPatcher.ReplaceFragment(opCodes, operands, instructions, replacement, "Patch_RestrictBillToPrisoner patch for DoWindowContents", false);

            List<CodeInstruction> instr = instructions.ToList();
            int start = 0;
            int end = 0;
            for (int i = 0; i < instr.Count(); i++)
            {
                if (instr[i].IsLdarg(0) && instr[i - 1].opcode == OpCodes.Stloc_S)
                {
                    if (instr[i - 1].operand is LocalBuilder lb1 && lb1.LocalIndex == 24)
                    {
                        start = i;

                    }
                }

                if (instr[i].IsStloc() && instr[i].operand is LocalBuilder lb && lb.LocalIndex == 25 && instr[i + 1].opcode == OpCodes.Ldloc_S)
                {
                    end = i;
                }
            }
            instr.RemoveRange(start, end - start + 1);
            instr.InsertRange(start, replacement);
            
            return instr.AsEnumerable();
        }
        public static string GetLabel(Dialog_BillConfig dialog)
        {

            Bill_Production bill = Traverse.Create(dialog).Field("bill").GetValue<Bill_Production>();
            if(bill.PawnRestriction != null)
            {
                return bill.PawnRestriction.LabelShortCap;
            }
            return GetDropLabel(dialog);

        }
        public static string GetDropLabel(Dialog_BillConfig dialog)
        {
            Log.Message("Get into patch");
            Bill_Production bill = Traverse.Create(dialog).Field("bill").GetValue<Bill_Production>();
            GroupMode groupMode = BillAssignationUtility.IsFor(bill);
            switch (groupMode)
            {
                case GroupMode.ColonyOnly:
                    return "PrisonLabor_ColonyOnly".Translate();
                case GroupMode.PrisonersOnly:
                    return "PrisonLabor_PrisonersOnly".Translate();
                case GroupMode.ColonistsOnly:
                    return "AnyWorker".Translate();
                case GroupMode.SlavesOnly:
                    return "AnySlave".Translate();
                case GroupMode.CaptiveOnly:
                    return "PrisonLabor_PrisonersAndSlaveOnly".Translate();
                default:
                    return (!ModsConfig.IdeologyActive || !bill.SlavesOnly) ? ("AnyWorker".Translate()) : ("AnySlave".Translate());
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("GeneratePawnRestrictionOptions")]
        static IEnumerable<Widgets.DropdownMenuElement<Pawn>> Postfix_GenerateFields(IEnumerable<Widgets.DropdownMenuElement<Pawn>> values, Dialog_BillConfig __instance)
        {
            int check = ModsConfig.IdeologyActive ? 1 : 0;
            int i = 0;
            Bill_Production bill = Traverse.Create(__instance).Field("bill").GetValue<Bill_Production>();
            Widgets.DropdownMenuElement<Pawn> anyone = new Widgets.DropdownMenuElement<Pawn>
            {
                option = new FloatMenuOption("PrisonLabor_ColonyOnly".Translate(), delegate
                {
                    Traverse.Create(bill).Field("slavesOnly").SetValue(true);
                    Traverse.Create(bill).Field("pawnRestriction").SetValue(null);
                    BillAssignationUtility.SetFor(bill, GroupMode.ColonyOnly);
                }),
                payload = null
            };
            yield return anyone;
            foreach (Widgets.DropdownMenuElement<Pawn> value in values)
            {
                yield return value;
                if (check == i)
                {
                    Widgets.DropdownMenuElement<Pawn> prisonerMenu = new Widgets.DropdownMenuElement<Pawn>
                    {
                        option = new FloatMenuOption("PrisonLabor_PrisonersOnly".Translate(), delegate
                        {
                            bill.SetAnyPawnRestriction();
                            BillAssignationUtility.SetFor(bill, GroupMode.PrisonersOnly);
                        }),
                        payload = null
                    };
                    yield return prisonerMenu;

                    Widgets.DropdownMenuElement<Pawn> anyCaptive = new Widgets.DropdownMenuElement<Pawn>
                    {
                        option = new FloatMenuOption("PrisonLabor_PrisonersAndSlaveOnly".Translate(), delegate
                        {
                            bill.SetAnySlaveRestriction();
                            BillAssignationUtility.SetFor(bill, GroupMode.CaptiveOnly);
                        }),
                        payload = null
                    };
                    yield return anyCaptive;
                }
                i++;
            }
            WorkGiverDef workGiver = bill.billStack.billGiver.GetWorkgiver();
            SkillDef workSkill = bill.recipe.workSkill;
            IEnumerable<Pawn> allPrisonersOfColony = PawnsFinder.AllMaps_PrisonersOfColony;

            allPrisonersOfColony = allPrisonersOfColony.OrderBy((Pawn pawn) => pawn.LabelShortCap);
            if (workSkill != null)
            {
                allPrisonersOfColony = allPrisonersOfColony.OrderByDescending((Pawn pawn) => pawn.skills.GetSkill(bill.recipe.workSkill).Level);
            }
            if (workGiver == null)
            {
                Log.ErrorOnce("Generating pawn restrictions for a BillGiver without a Workgiver", 96455148);
                yield break;
            }
            allPrisonersOfColony = allPrisonersOfColony.OrderByDescending((Pawn pawn) => pawn.workSettings.WorkIsActive(workGiver.workType));
            allPrisonersOfColony = allPrisonersOfColony.OrderBy((Pawn pawn) => pawn.WorkTypeIsDisabled(workGiver.workType));

            Widgets.DropdownMenuElement<Pawn> dropdownMenuElement;
            foreach (Pawn pawn in allPrisonersOfColony)
            {
                if (PrisonLaborUtility.LaborEnabled(pawn))
                {
                    if (pawn.WorkTypeIsDisabled(workGiver.workType))
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption(string.Format("P: {0} ({1})", pawn.LabelShortCap, "WillNever".Translate(workGiver.verb)), null),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                    else if (bill.recipe.workSkill != null && !pawn.workSettings.WorkIsActive(workGiver.workType))
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption(string.Format("P: {0} ({1} {2}, {3})", pawn.LabelShortCap, pawn.skills.GetSkill(bill.recipe.workSkill).Level, bill.recipe.workSkill.label, "NotAssigned".Translate()), delegate
                            {
                                bill.SetPawnRestriction(pawn);
                            }),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                    else if (!pawn.workSettings.WorkIsActive(workGiver.workType))
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption(string.Format("P: {0} ({1})", pawn.LabelShortCap, "NotAssigned".Translate()), delegate
                            {
                                bill.SetPawnRestriction(pawn);
                            }),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                    else if (bill.recipe.workSkill != null)
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption($"P: {pawn.LabelShortCap} ({pawn.skills.GetSkill(bill.recipe.workSkill).Level} {bill.recipe.workSkill.label})", delegate
                            {
                                bill.SetPawnRestriction(pawn);
                            }),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                    else
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption($"P: {pawn.LabelShortCap}", delegate
                            {
                                bill.SetPawnRestriction(pawn);
                            }),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                }
            }
        }
    }

}
