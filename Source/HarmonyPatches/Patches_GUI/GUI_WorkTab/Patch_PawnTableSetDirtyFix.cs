using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PrisonLabor.Core.MainButton_Window;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_WorkTab
{
    /// <summary>
    /// This partch is ensuring prisonersTable is set dirty, when parent component is set to dirty too.
    /// </summary>
    [HarmonyPatch(typeof(MainTabWindowUtility))]
    [HarmonyPatch("NotifyAllPawnTables_PawnsChanged")]
    internal class Patch_PawnTableSetDirtyFix
    {
        private static void PostFix()
        {
            WindowNotifier.NotifyPLWindows();
        }
    }

    [HarmonyPatch(typeof(Pawn_GuestTracker))]
    [HarmonyPatch("SetGuestStatus")]
    internal class Patch_NotifyWindowsWhenGetPrisoners{
        static void Postfix()
        {
            WindowNotifier.NotifyPLWindows();
        }
     }
}