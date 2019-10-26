using Harmony;
using PrisonLabor.Core.LaborArea;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_LaborArea
{
    [HarmonyPatch(typeof(AreaManager))]
    [HarmonyPatch("AddStartingAreas")]
    [HarmonyPatch]
    internal class AddLaborAreaPatch
    {
        private static void Postfix(AreaManager __instance)
        {
            __instance.AllAreas.Add(new Area_Labor(__instance));
        }
    }
}