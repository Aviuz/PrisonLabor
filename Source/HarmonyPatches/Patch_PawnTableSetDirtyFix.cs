using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    /// <summary>
    /// This partch is ensuring prisonersTable is set dirty, when parent component is set to dirty too.
    /// </summary>
    [HarmonyPatch(typeof(MainTabWindow_PawnTable))]
    [HarmonyPatch("SetDirty")]
    internal class Patch_PawnTableSetDirtyFix
    {
        private static void Prefix(MainTabWindow_PawnTable __instance)
        {
            var prisonersTable = __instance.GetType().GetField("prisonersTable", BindingFlags.NonPublic | BindingFlags.Instance);
            if (prisonersTable != null)
            {
                var SetDirty = prisonersTable.FieldType.GetMethod("SetDirty");
                if (SetDirty != null)
                    SetDirty.Invoke(prisonersTable.GetValue(__instance), new object[] { });
            }
        }
    }
}