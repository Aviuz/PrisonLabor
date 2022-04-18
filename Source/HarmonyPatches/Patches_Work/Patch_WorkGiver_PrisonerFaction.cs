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
    class Patch_WorkGiver_PrisonerFaction
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return Assembly.GetAssembly(typeof(WorkGiver_Scanner)).GetTypes()
                        .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(WorkGiver_Scanner)))
                        .SelectMany(type => type.GetMethods())
                            .Where(method => method.Name.Equals("PotentialWorkThingsGlobal") || method.Name.Equals("ShouldSkip") || method.Name.Equals("HasJobOnThing"))
                            .Distinct()
                            .Cast<MethodBase>();
        }

        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> inst)
        {
            var codes = new List<CodeInstruction>(inst);
            for (int i = 0; i < codes.Count(); i++)
            {
                if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
                {
                    DebugLogger.debug($"WorkThingsGlobal & ShouldSkip patch: {mBase.ReflectedType.Name}.{mBase.Name}");
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
            return prev.opcode == OpCodes.Ldarg_1 && actual.opcode == OpCodes.Callvirt && actual.operand != null && actual.operand.ToString().Contains("RimWorld.Faction get_Faction()");
        }
    }
}
