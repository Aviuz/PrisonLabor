using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_AssignBed
{
  [HarmonyPatch(typeof(Building_Bed))]
  [HarmonyPatch(nameof(Building_Bed.GetGizmos))]
  internal static class Patch_AssignPrisonersToBed
  {
    private static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Building_Bed __instance)
    {
      foreach (var gizmo in __result) yield return gizmo;

      if (__instance.ForPrisoners)
        yield return new Command_Action
        {
          defaultLabel = "PrisonLabor_CommandBedSetOwnerLabel".Translate(),
          defaultDesc = "PrisonLabor_CommandBedSetOwnerDesc".Translate(),
          icon = ContentFinder<Texture2D>.Get("ui/commands/AssignOwner"),
          action = () => Find.WindowStack.Add(new Dialog_AssignBuildingOwner(__instance.CompAssignableToPawn))
        };
    }
  }


  [HarmonyPatch(typeof(CompAssignableToPawn_Bed))]
  [HarmonyPatch("get_" + nameof(CompAssignableToPawn.AssigningCandidates))]
  internal static class Patch_MakePrisonersCandidates
  {
    private static bool Prefix(ref IEnumerable<Pawn> __result, CompAssignableToPawn __instance)
    {
      var bed = __instance.parent as Building_Bed;
      if (bed != null && bed.Spawned && __instance is CompAssignableToPawn_Bed && bed.ForPrisoners)
      {
        __result = bed.Map.mapPawns.PrisonersOfColony;
        return false;
      }

      return true;
    }
  }


  [HarmonyPatch(typeof(WorkGiver_Warden_TakeToBed))]
  [HarmonyPatch("TakeToPreferredBedJob")]
  internal static class Patch_TakePrisonersToOwnedBed
  {
    /*  === Orignal code Look-up===
     *
     *  if (RestUtility.FindBedFor(prisoner, prisoner, true, true, false) != null)
     *  {
     *  	return null;
     *  }
     *
     *  === CIL Instructions ===
     *
     *  ldarg.1 |  | Label 2
     *  ldarg.1 |  | no labels
     *  ldc.i4.1 |  | no labels
     *  ldc.i4.1 |  | no labels
     *  ldc.i4.0 |  | no labels
     *  call | RimWorld.Building_Bed FindBedFor(Verse.Pawn, Verse.Pawn, Boolean, Boolean, Boolean) | no labels
     *  brfalse | Label 3 | no labels
     */

    private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
      IEnumerable<CodeInstruction> instructions)
    {
      OpCode[] opCodes1 =
      {
        OpCodes.Ldarg_0,
        OpCodes.Ldarg_0,
        OpCodes.Ldc_I4_1,
        OpCodes.Ldc_I4_0,
        OpCodes.Ldc_I4_1,
        OpCodes.Newobj,
        OpCodes.Call,
        OpCodes.Brfalse_S
      };
      string[] operands1 =
      {
        "",
        "",
        "",
        "",
        "",
        "Void .ctor(RimWorld.GuestStatus)",
        "RimWorld.Building_Bed FindBedFor(Verse.Pawn, Verse.Pawn, Boolean, Boolean, System.Nullable`1[RimWorld.GuestStatus])",
        "System.Reflection.Emit.Label"
      };
      var step1 = 0;

      var label_OriginalBranch = gen.DefineLabel();
      foreach (var instr in instructions)
      {
        if (HPatcher.IsFragment(opCodes1, operands1, instr, ref step1, nameof(Patch_TakePrisonersToOwnedBed)))
        {
          yield return new CodeInstruction(OpCodes.Ldarg_1);
          yield return new CodeInstruction(OpCodes.Call,
            typeof(Patch_TakePrisonersToOwnedBed).GetMethod(nameof(HaveOwnedBed)));
          yield return new CodeInstruction(OpCodes.Brfalse, label_OriginalBranch);
          yield return new CodeInstruction(OpCodes.Pop);
          yield return new CodeInstruction(OpCodes.Ldarg_1);
          yield return new CodeInstruction(OpCodes.Call,
            typeof(Patch_TakePrisonersToOwnedBed).GetMethod(nameof(CanReachBed)));
          yield return new CodeInstruction(OpCodes.Brfalse, instr.operand);
          yield return new CodeInstruction(OpCodes.Ldnull);
          yield return new CodeInstruction(OpCodes.Ret);

          instr.labels.Add(label_OriginalBranch);
        }

        yield return instr;
      }
    }

    public static bool HaveOwnedBed(Pawn pawn)
    {
      return pawn.ownership?.OwnedBed != null;
    }

    public static bool CanReachBed(Pawn pawn)
    {
      return pawn.CanReach(pawn.ownership.OwnedBed, PathEndMode.OnCell, Danger.Some);
    }
  }
}