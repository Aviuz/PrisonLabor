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
using Therapy;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Construction
{
    [HarmonyPatch(typeof(WorkGiver_ReceiveTherapy), "HasJobOnThing")]
    class Patch_WorkGiver_ReceiveTherapy
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> inst)
        {
            var codes = new List<CodeInstruction>(inst);
            for (int i = 0; i < codes.Count(); i++)
            {
                if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
                {
                    DebugLogger.debug($"Therapy HasJobOnThing patch: {mBase.ReflectedType.Name}.{mBase.Name}");
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
