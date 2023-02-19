using HarmonyLib;
using RimWorld;
using PrisonLabor.Core.BillAssignation;
using PrisonLabor.HarmonyPatches.Patches_GUI.GUI_Bill;
using TacticalGroups;
using System.Collections.Generic;
using Verse;
using PrisonLabor.Core.Other;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using static Verse.Widgets;
using System;

namespace ColonyGroupsCompatibility.HarmonyPatches
{
  [HarmonyPatch(typeof(Patch_RestrictBillToPrisoner))]
  public class ReturnLabel_Patch
  {
    [HarmonyPostfix]
    [HarmonyPatch("GetDropLabel")]
    public static string PostfixGetDropLabel(string __result, Dialog_BillConfig dialog)
    {
      Bill_Production bill = Traverse.Create(dialog).Field("bill").GetValue<Bill_Production>();
      if (BillAssignationUtility.IsFor(bill) == GroupMode.ColonyGroups)
      {
        if (HarmonyPatches_GroupBills.BillsSelectedGroup.TryGetValue(bill, out PawnGroup group))
        {
          string label = "TG.AnyPawnOfGroup".Translate(group.curGroupName);
          return label;
        }
      }
      return __result;
    }

  }
  [HarmonyPatch]
  public class MarkGroupAssigment_Patch
  {
    static MethodBase TargetMethod()
    {
      return AccessTools.Method("TacticalGroups.HarmonyPatches_GroupBills:GeneratePawnRestrictionOptions");
    }
    public static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> inst)
    {
      int bill = -1;
      var codes = new List<CodeInstruction>(inst);
      for (int i = 0; i < codes.Count(); i++)
      {
        yield return codes[i];
        if (ShouldCopy(codes[i]))
        {
          bill = i;
        }
        if (ShouldPatch(codes[i]) && bill > -1)
        {
          DebugLogger.debug($"Changing delegate: {mBase.ReflectedType.Assembly.GetName().Name}.{mBase.ReflectedType.Name}.{mBase.Name}");
          yield return new CodeInstruction(OpCodes.Ldloc_3, null);
          //ldfld | TacticalGroups.HarmonyPatches_GroupBills+<>c__DisplayClass6_0 CS$<>8__locals1 | no labels
          yield return codes[bill - 1];
          //ldfld | RimWorld.Bill_Production ___bill | no labels
          yield return codes[bill];
          //ldloc.s | Verse.Widgets+DropdownMenuElement`1[Verse.Pawn] (8) | no labels
          yield return codes[i - 1];
          yield return new CodeInstruction(OpCodes.Call, typeof(MarkGroupAssigment_Patch).GetMethod(nameof(MarkGroupAssigment_Patch.UpdateAction)));
        }
      }
    }

    private static bool ShouldPatch(CodeInstruction actual)
    {
      return actual.opcode == OpCodes.Callvirt && actual.operand != null && actual.operand.ToString().Contains("Void Insert(Int32, DropdownMenuElement`1)");
    }

    private static bool ShouldCopy(CodeInstruction actual)
    {
      return actual.opcode == OpCodes.Ldfld && actual.operand != null && actual.operand.ToString().Contains("RimWorld.Bill_Production ___bill");
    }
    public static void UpdateAction(Bill_Production bill, DropdownMenuElement<Pawn> element)
    {
      Action notify = delegate
      {
        BillAssignationUtility.SetFor(bill, GroupMode.ColonyGroups);
        DebugLogger.debug($"For {bill.Label} set {GroupMode.ColonyGroups}");
      };
      element.option.action += notify;
    }
  }
}
