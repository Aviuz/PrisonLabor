using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

#pragma warning disable CS0252

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Dialog_BillConfig))]
    [HarmonyPatch("DoWindowContents")]
    [HarmonyPatch(new[] {typeof(Rect)})]
    internal class Patch_BillCheckbox
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            var step = 0;
            CodeInstruction loadBillInstr = null;
            var label = new Label();
            foreach (var instruction in instr)
            {
                if (step == 0)
                    if (instruction.opcode == OpCodes.Ldfld)
                    {
                        loadBillInstr = instruction;
                        step++;
                    }
                if (step == 1)
                {
                    if (instruction.opcode == OpCodes.Ldstr && instruction.operand == "BillStoreMode_")
                        step++;
                }
                else if (step == 2)
                {
                    if (instruction.opcode == OpCodes.Brfalse)
                    {
                        label = (Label) instruction.operand;
                        step++;
                    }
                }
                else if (step == 3)
                {
                    if (instruction.labels.Count != 0 && instruction.labels.Contains(label))
                    {
                        var injectedInstruction = new CodeInstruction(OpCodes.Ldloc_S, instruction.operand);
                        injectedInstruction.labels.Add(label);
                        yield return injectedInstruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(loadBillInstr);
                        yield return new CodeInstruction(OpCodes.Call,
                            typeof(Patch_BillCheckbox).GetMethod("GroupExclusionButton"));
                        instruction.labels.Remove(label);
                    }
                }
                yield return instruction;
            }
        }

        public static void GroupExclusionButton(Listing_Standard listing, Bill bill)
        {
            if (BillUtility.IsFor(bill) == GroupMode.ColonistsOnly)
            {
                if (listing.ButtonText("Colonists only"))
                    BillUtility.SetFor(bill, GroupMode.PrisonersOnly);
            }
            else if (BillUtility.IsFor(bill) == GroupMode.PrisonersOnly)
            {
                if (listing.ButtonText("Prisoners only"))
                    BillUtility.SetFor(bill, GroupMode.ColonyOnly);
            }
            else
            {
                if (listing.ButtonText("Colony only"))
                    BillUtility.SetFor(bill, GroupMode.ColonistsOnly);
            }
            listing.Gap(12f);
        }
    }
}