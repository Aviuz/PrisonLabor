using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(WidgetsWork))]
    [HarmonyPatch("DrawWorkBoxFor")]
    [HarmonyPatch(new Type[] { typeof(float), typeof(float), typeof(Pawn), typeof(WorkTypeDef), typeof(bool) })]
    class WorkDisablePatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            // Define label to the begining of the original code
            Label jumpTo = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Ldarg_3);
            yield return new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod("WorkDisabled", new Type[] { typeof(Pawn), typeof(WorkTypeDef) }));
            //If false continue
            yield return new CodeInstruction(OpCodes.Brfalse, jumpTo);
            //Return
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach (CodeInstruction ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                yield return ci;
            }
        }
    }
}
