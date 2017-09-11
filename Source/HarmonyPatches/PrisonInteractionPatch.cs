using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(PrisonerInteractionModeUtility))]
    [HarmonyPatch("GetLabel")]
    [HarmonyPatch(new[] {typeof(PrisonerInteractionModeDef)})]
    internal class PrisonInteractionPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            // create our WORK label
            var jumpTo = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, typeof(PrisonInteractionPatch).GetMethod("getLabelWork"));
            //yield return new CodeInstruction(OpCodes.Dup);
            yield return new CodeInstruction(OpCodes.Ldstr, "Work");
            yield return new CodeInstruction(OpCodes.Bne_Un, jumpTo);
            yield return new CodeInstruction(OpCodes.Ldstr, "PrisonLabor_PrisonerWork".Translate());
            yield return new CodeInstruction(OpCodes.Ret);

            var first = true;
            foreach (var ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                //debug
                //Log.Message("CODE: ToString():" + ci.ToString() + " || labels:" + ci.labels.Any());
                yield return ci;
            }
        }

        public static string getLabelWork(PrisonerInteractionModeDef def)
        {
            if (def == DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption"))
                return "Work";
            return "";
        }
    }
}