using Harmony;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(AreaManager))]
    [HarmonyPatch("ExposeData")]
    internal class Patch_RemoveLaborAreaIfDisabled
    {
        private static void Prefix(AreaManager __instance)
        {
            if (PrisonLaborPrefs.DisableMod)
            {
                __instance.AllAreas.RemoveAll(area => area is Area_Labor);
            }
        }
    }
}
