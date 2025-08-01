using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using PrisonLabor.Core.Other;
using System.Reflection.Emit;

namespace CleaningAreaCompatibility.HarmonyPatches
{
    [HarmonyPatch]
    class CleanAreaPatch
    {
        static MethodBase TargetMethod()
        {
            var modName = ModsConfig.IsActive("s1.cleaningareatemp") ? "CleaningAreaTemp" : "CleaningArea";
            return AccessTools.Method($"{modName}.WorkGiver_CleanFilth_CleaningArea:HasJobOnThing");
        }
        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> inst)
        {
            var codes = new List<CodeInstruction>(inst);
            for (int i = 0; i < codes.Count; i++)
            {
                if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
                {
                    DebugLogger.debug($"WorkThingsGlobal & ShouldSkip patch: {mBase.ReflectedType.Assembly.GetName().Name}.{mBase.ReflectedType.Name}.{mBase.Name}");
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
