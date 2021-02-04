using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_Bill
{
    [HarmonyPatch(typeof(Dialog_BillConfig))]
    [HarmonyPatch("GeneratePawnRestrictionOptions")]
    class Patch_RestrictBillToPrisoner
    {
        static IEnumerable<Widgets.DropdownMenuElement<Pawn>> Postfix(IEnumerable<Widgets.DropdownMenuElement<Pawn>> values,  Dialog_BillConfig __instance)
        {
            foreach (Widgets.DropdownMenuElement<Pawn> value in values)
            {
                yield return value;
            }
            Bill_Production bill = Traverse.Create(__instance).Field("bill").GetValue<Bill_Production>();			
            WorkGiverDef workGiver = bill.billStack.billGiver.GetWorkgiver();
            List<Pawn> allPrisonersOfColony = PawnsFinder.AllMaps_PrisonersOfColony;
            Widgets.DropdownMenuElement<Pawn> dropdownMenuElement;
            foreach (Pawn pawn in allPrisonersOfColony)
            {
                if (PrisonLaborUtility.LaborEnabled(pawn))
                {
                    if (pawn.WorkTypeIsDisabled(workGiver.workType))
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption(string.Format("{0} ({1})", pawn.LabelShortCap, "WillNever".Translate(workGiver.verb)), null),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                    else if (bill.recipe.workSkill != null && !pawn.workSettings.WorkIsActive(workGiver.workType))
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption(string.Format("{0} ({1} {2}, {3})", pawn.LabelShortCap, pawn.skills.GetSkill(bill.recipe.workSkill).Level, bill.recipe.workSkill.label, "NotAssigned".Translate()), delegate
                            {
                                bill.pawnRestriction = pawn;
                            }),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                    else if (!pawn.workSettings.WorkIsActive(workGiver.workType))
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption(string.Format("{0} ({1})", pawn.LabelShortCap, "NotAssigned".Translate()), delegate
                            {
                                bill.pawnRestriction = pawn;
                            }),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                    else if (bill.recipe.workSkill != null)
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption($"{pawn.LabelShortCap} ({pawn.skills.GetSkill(bill.recipe.workSkill).Level} {bill.recipe.workSkill.label})", delegate
                            {
                                bill.pawnRestriction = pawn;
                            }),
                            payload = pawn
                        };
                        yield return dropdownMenuElement;
                    }
                    else
                    {
                        dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>
                        {
                            option = new FloatMenuOption($"{pawn.LabelShortCap}", delegate
                            {
                                bill.pawnRestriction = pawn;
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
