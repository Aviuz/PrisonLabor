using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_LaborArea
{
  [HarmonyPatch]
  class Patch_Labor_Position
  {

    static IEnumerable<MethodBase> TargetMethods()
    {
      return Assembly.GetAssembly(typeof(WorkGiver_Scanner)).GetTypes()
      .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(WorkGiver_Scanner)))
      .SelectMany(type => type.GetMethods())
          .Where(method => method.Name.Equals("PotentialWorkCellsGlobal"))
          .Cast<MethodBase>();
    }
    static IEnumerable<IntVec3> Postfix(IEnumerable<IntVec3> __result, WorkGiver_Scanner __instance, Pawn pawn)
    {
      if (__result != null && __instance != null)
      {
        return __result.Where(item => PrisonLaborUtility.CanWorkHere(item, pawn, __instance.def.workType));
      }
      else
      {
        return __result;
      }
    }
  }
}
