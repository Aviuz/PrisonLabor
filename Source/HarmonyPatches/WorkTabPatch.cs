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
    [HarmonyPatch(typeof(MainTabWindow_Work))]
    [HarmonyPatch("get_Pawns")]
    class WorkTabPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, typeof(WorkTabPatch).GetMethod("Pawns"));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        public static IEnumerable<Pawn> Pawns(MainTabWindow mainTabWindow)
        {
            foreach (Pawn p in Find.VisibleMap.mapPawns.FreeColonists)
                yield return p;
            if (mainTabWindow is MainTabWindow_Work || mainTabWindow is MainTabWindow_Restrict || mainTabWindow.GetType().ToString().Contains("MainTabWindow_WorkTab"))
            {
                foreach (Pawn pawn in Find.VisibleMap.mapPawns.PrisonersOfColony)
                    if (PrisonLaborUtility.LaborEnabled(pawn))
                    {
                        PrisonLaborUtility.InitWorkSettings(pawn);
                        yield return pawn;
                    }
            }
        }
    }
}
