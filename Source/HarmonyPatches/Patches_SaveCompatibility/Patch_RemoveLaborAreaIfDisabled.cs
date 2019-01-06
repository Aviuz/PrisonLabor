using Harmony;
using PrisonLabor.Core.LaborArea;
using PrisonLabor.Core.Meta;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_SaveCompatibility
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
