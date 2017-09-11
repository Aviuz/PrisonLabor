using Harmony;
using Verse;

namespace PrisonLabor.Harmony
{
    [HarmonyPatch(typeof(AreaManager))]
    [HarmonyPatch("AddStartingAreas")]
    [HarmonyPatch]
    internal class AddLaborAreaPatch
    {
        public static void Postfix(AreaManager __instance)
        {
            __instance.AllAreas.Add(new Area_Labor(__instance));
        }
    }
}