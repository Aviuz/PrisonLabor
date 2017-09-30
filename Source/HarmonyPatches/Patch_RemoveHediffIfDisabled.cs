using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("ExposeData")]
    class Patch_RemoveHediffIfDisabled
    {
        private static void Prefix(Pawn __instance)
        {
            if (PrisonLaborPrefs.DisableMod)
            {
                if (__instance.health != null && __instance.health.hediffSet != null)
                {
                    var hediff = __instance.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("PrisonLabor_PrisonerChains"), false);
                    if (hediff != null)
                        __instance.health.RemoveHediff(hediff);
                }
            }
        }
    }
}
