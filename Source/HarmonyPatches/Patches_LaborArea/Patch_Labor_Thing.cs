using HarmonyLib;
using MonoMod.Utils;
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
  class Patch_Labor_Thing
  {
    static IEnumerable<MethodBase> TargetMethods()
    {
      foreach (var method in GetBaseMethods())
      {
        yield return method;
      }
      // yield return getCleanMethod();
    }
    static IEnumerable<MethodBase> GetBaseMethods()
    {
      return Assembly.GetAssembly(typeof(WorkGiver_Scanner)).GetTypes()
      .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(WorkGiver_Scanner)) && myType != typeof(WorkGiver_Teach))
      .SelectMany(type => type.GetMethods())
          .Where(method => method.Name.Equals("PotentialWorkThingsGlobal"))
          .Cast<MethodBase>();
    }

    static MethodBase getCleanMethod()
    {
      Assembly asm = typeof(WorkGiver_Scanner).Assembly;
      Type type = asm.GetType("RimWorld.WorkGiver_CleanFilth");
      return type.GetMethod("PotentialWorkThingsGlobal");
    }

    static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, WorkGiver_Scanner __instance, Pawn pawn)
    {
      if (__result != null && __instance != null)
      {
        return __result.Where(item => item is Thing thing && PrisonLaborUtility.CanWorkHere(thing.Position, pawn, __instance.def.workType));
      }
      else
      {
        return __result;
      }
    }
  }
}
