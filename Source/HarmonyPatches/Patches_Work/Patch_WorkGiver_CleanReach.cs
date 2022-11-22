using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using System.Reflection;
using PrisonLabor.Core;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch]
  class Patch_WorkGiver_CleanReach
  {
    static MethodBase TargetMethod()
    {
      Assembly asm = typeof(WorkGiver_Scanner).Assembly;
      return asm.GetType("RimWorld.WorkGiver_CleanFilth").GetMethod("HasJobOnThing");
    }

    static bool Postfix(bool __result, Pawn pawn, Thing t, bool forced)
    {
      if (__result && pawn.IsPrisonerOfColony)
      {
        return __result && pawn.CanReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger());
      }
      return __result;
    }

  }
}
