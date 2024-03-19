using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch]
  public class Patch_WorkGiver_PrisonerFaction
  {
    static IEnumerable<MethodBase> TargetMethods()
    {
      foreach (MethodBase mb in Assembly.GetAssembly(typeof(WorkGiver_Scanner)).GetTypes()
                  .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(WorkGiver_Scanner)))
                  .SelectMany(type => type.GetMethods())
                      .Where(method => method.Name.Equals("PotentialWorkThingsGlobal") || method.Name.Equals("ShouldSkip") || method.Name.Equals("HasJobOnThing"))
                      .Distinct()
                      .Cast<MethodBase>())
      {
        yield return mb;
      }
      yield return typeof(WorkGiver_ConstructFinishFrames).GetMethod(nameof(WorkGiver_ConstructFinishFrames.JobOnThing));
      yield return typeof(WorkGiver_ConstructDeliverResourcesToFrames).GetMethod(nameof(WorkGiver_ConstructDeliverResourcesToFrames.JobOnThing));
      yield return typeof(WorkGiver_ConstructDeliverResourcesToBlueprints).GetMethod(nameof(WorkGiver_ConstructDeliverResourcesToBlueprints.JobOnThing));
      yield return typeof(ChildcareUtility).GetMethod(nameof(ChildcareUtility.HasBreastfeedCompatibleFactions), new[] { typeof(Faction), typeof(Pawn) });
    }

    public static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> inst)
    {     
      var codes = new List<CodeInstruction>(inst);
      for (int i = 0; i < codes.Count(); i++)
      {
        if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
        {
          DebugLogger.debug($"Patch_WorkGiver_PrisonerFaction patch: {mBase.ReflectedType.Assembly.GetName().Name}.{mBase.ReflectedType.Name}.{mBase.Name}");
          yield return new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod(nameof(PrisonLaborUtility.GetPawnFaction)));
        }
        else
        {
          yield return codes[i];
        }
      }
    }

    private static bool ShouldPatch(CodeInstruction actual, CodeInstruction prev)
    {
      return prev.opcode == OpCodes.Ldarg_1 && HPatcher.IsGetFactionOperand(actual);
    }
  }
}
