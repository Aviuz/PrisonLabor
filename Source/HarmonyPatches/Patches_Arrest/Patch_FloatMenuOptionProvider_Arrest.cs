using System;
using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Arrest
{
  [HarmonyPatch(typeof(FloatMenuOptionProvider_Arrest))]
  [HarmonyPatch("GetSingleOptionFor")]
  [HarmonyPatch(new[] { typeof(Pawn), typeof(FloatMenuContext) })]
  public class Patch_FloatMenuOptionProvider_Arrest
  {
    private static FloatMenuOption Postfix(FloatMenuOption __result, Pawn clickedPawn, FloatMenuContext context)
    {
      if (__result == null && context.FirstSelectedPawn.CanReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly) && 
        clickedPawn.IsPrisonerOfColony && ArrestUtility.IsPawnFleeing(clickedPawn))
      {
        
        string label = "TryToArrest".Translate(clickedPawn.LabelCap, clickedPawn,
          clickedPawn.GetAcceptArrestChance(context.FirstSelectedPawn).ToStringPercent());
        Action action = delegate {ArrestUtility.ArrestPrisoner(clickedPawn, context.FirstSelectedPawn); };
        var priority = MenuOptionPriority.High;
        return FloatMenuUtility.DecoratePrioritizedTask(
          new FloatMenuOption(label, action, priority, null, clickedPawn), context.FirstSelectedPawn, clickedPawn);
      }
      return __result;
    }
  }
}