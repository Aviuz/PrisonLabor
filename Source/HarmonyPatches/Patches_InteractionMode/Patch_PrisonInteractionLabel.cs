using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_InteractionMode
{
    //[HarmonyPatch(typeof(PrisonerInteractionModeUtility))]
    //[HarmonyPatch("GetLabel")]
    //[HarmonyPatch(new[] {typeof(PrisonerInteractionModeDef)})]
    internal class Patch_PrisonInteractionLabel
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
            IEnumerable<CodeInstruction> instr)
        {
            // create our WORK label
            var jumpTo = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, typeof(Patch_PrisonInteractionLabel).GetMethod(nameof(getLabelWork)));
            yield return new CodeInstruction(OpCodes.Brfalse, jumpTo);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, typeof(Patch_PrisonInteractionLabel).GetMethod(nameof(getLabelWork)));
            yield return new CodeInstruction(OpCodes.Ret);

            var first = true;
            foreach (var ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                yield return ci;
            }
        }

        public static string getLabelWork(PrisonerInteractionModeDef def)
        {
            //TODO add translation to work and recruit
            if (def == DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption"))
                return "PrisonLabor_PrisonerWork".Translate();
            else if (def == DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workAndRecruitOption"))
                return "PrisonLabor_WorkAndRecruit".Translate();
            return null;
        }
    }
}