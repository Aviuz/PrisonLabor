using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;

#pragma warning disable CS0252

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Dialog_BillConfig))]
    [HarmonyPatch("DoWindowContents")]
    [HarmonyPatch(new Type[] { typeof(Rect) })]
    class BillCheckboxPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            int step = 0;
            CodeInstruction loadBillInstr = null;
            Label label = new Label();
            foreach (CodeInstruction instruction in instr)
            {
                if (step == 0)
                {
                    if (instruction.opcode == OpCodes.Ldfld)
                    {
                        loadBillInstr = instruction;
                        step++;
                    }
                }
                if (step == 1)
                {
                    if (instruction.opcode == OpCodes.Ldstr && instruction.operand == "BillStoreMode_")
                    {
                        step++;
                    }
                }
                else if (step == 2)
                {
                    if (instruction.opcode == OpCodes.Brfalse)
                    {
                        label = (Label)instruction.operand;
                        step++;
                    }
                }
                else if (step == 3)
                {
                    if (instruction.labels.Count != 0 && instruction.labels.Contains(label))
                    {
                        CodeInstruction injectedInstruction = new CodeInstruction(OpCodes.Ldloc_S, instruction.operand);
                        injectedInstruction.labels.Add(label);
                        yield return injectedInstruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(loadBillInstr);
                        yield return new CodeInstruction(OpCodes.Call, typeof(BillCheckboxPatch).GetMethod("GroupExclusionButton"));
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
