using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_Feeding
{
  [HarmonyPatch(typeof(ITab_Pawn_Feeding))]
  class Patch_ITAB_Pawn_Feeding
  {
    [HarmonyPatch("FillTab")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(Rect), typeof(Vector2), typeof(Vector2), typeof(List<Pawn>) },
        new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal })]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> FillTabTranspiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
    {
      var expected = AccessTools.PropertyGetter(typeof(PawnsFinder), nameof(PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_OfPlayerFaction));
      foreach (CodeInstruction inst in instr)
      {
        if (inst.OperandIs(expected))
        {
          yield return new CodeInstruction(OpCodes.Call, typeof(Patch_ITAB_Pawn_Feeding).GetMethod(nameof(Patch_ITAB_Pawn_Feeding.GetNewRange)));
        }
        else
        {
          yield return inst;

        }
      }
    }

    [HarmonyPatch]
    [HarmonyPatch("DrawRow")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> DrawRowTranspiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
    {
      var expected = AccessTools.PropertyGetter(typeof(Entity), nameof(Entity.LabelShortCap));

      foreach (CodeInstruction inst in instr)
      {
        if (inst.OperandIs(expected))
        {
          yield return new CodeInstruction(OpCodes.Call, typeof(Patch_ITAB_Pawn_Feeding).GetMethod(nameof(Patch_ITAB_Pawn_Feeding.HandlePrisonerLabel)));
        }
        else
        {
          yield return inst;
        }    
      }
    }

    public static string HandlePrisonerLabel(Pawn feeder)
    {
      if (feeder.IsPrisonerOfColony)
      {
        return "Prisoner " + feeder.LabelShortCap;
      }
      return feeder.LabelShortCap;
    }

    public static List<Pawn> GetNewRange()
    {
      List<Pawn> list = new List<Pawn>(PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_OfPlayerFaction);
      list.AddRange(PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_PrisonersOfColony);
      return list;
    }
  }
}
