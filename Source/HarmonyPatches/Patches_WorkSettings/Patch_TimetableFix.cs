using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using System.Reflection.Emit;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_WorkSettings
{
  [HarmonyPatch(typeof(Pawn_TimetableTracker))]
  [HarmonyPatch("get_CurrentAssignment")]
  internal class Patch_TimetableFix
  {
    static TimeAssignmentDef Postfix(TimeAssignmentDef __result, Pawn_TimetableTracker __instance)
    {
      Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
      if (pawn.IsPrisonerOfColony)
      {
        return __instance.times[GenLocalDate.HourOfDay(pawn)];
      }
      return __result;
    }
  }
}
