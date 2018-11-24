using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    static class Patch_EscapeTracker
    {
        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch(nameof(Pawn.ExposeData))]
        static class Patch_ExposeData
        {
            static void Postfix(Pawn __instance)
            {
                var escapeTracker = EscapeTracker.Of(__instance);
                Scribe_Deep.Look(ref escapeTracker, "EscapeTracker", new object[] { __instance});
                EscapeTracker.Save(__instance, escapeTracker);
            }
        }

        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch(nameof(Pawn.Tick))]
        static class Patch_Tick
        {
            static void Postfix(Pawn __instance)
            {
                if (!__instance.Dead)
                {
                    EscapeTracker.Of(__instance)?.Tick();
                }
            }
        }
    }
}
