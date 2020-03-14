using HarmonyLib;
using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_SaveCompatibility
{
    [HarmonyPatch(typeof(Pawn_WorkSettings))]
    [HarmonyPatch("ExposeData")]
    static internal class Patch_JailorTypeSaveCompatibility
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            // Find >> priorities
            var priorities = HPatcher.FindOperandAfter(new[] { OpCodes.Ldflda }, new[] { "Verse.DefMap`2[Verse.WorkTypeDef,System.Int32] priorities" }, instr);

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Dup);
            yield return new CodeInstruction(OpCodes.Ldfld, priorities);
            yield return new CodeInstruction(OpCodes.Call, typeof(Patch_JailorTypeSaveCompatibility).GetMethod(nameof(AddJailor)));
            //yield return new CodeInstruction(OpCodes.Pop);
            //yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Stfld, priorities);
            foreach (var ci in instr)
            {
                yield return ci;
            }
        }

        public static DefMap<WorkTypeDef, int> AddJailor(DefMap<WorkTypeDef, int> priorities)
        {
            try
            {
                if (priorities != null)
                {
                    var check = priorities[DefDatabase<WorkTypeDef>.AllDefs.Max(d => d.index)];
                    return priorities;
                }
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                var newPriorities = new DefMap<WorkTypeDef, int>();

                int jailorIndex = PL_DefOf.PrisonLabor_Jailor.index;
                newPriorities[PL_DefOf.PrisonLabor_Jailor] = 0;
                foreach (var def in DefDatabase<WorkTypeDef>.AllDefs.Where(d => d.index < priorities.Count))
                {
                    if (def.index < jailorIndex)
                        newPriorities[def] = priorities[def];
                    else if (def.index > jailorIndex)
                        newPriorities[def] = priorities[def.index - 1];
                }

                return newPriorities;
            }
        }
    }
}
