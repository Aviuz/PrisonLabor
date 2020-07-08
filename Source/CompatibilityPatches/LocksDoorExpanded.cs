
using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.CompatibilityPatches
{
    internal static class LocksDoorExpanded
    {
        private static bool foundType;

        public static bool Found => foundType;

        internal static void Init()
        {
            if (Check())
            {
                // TODO:
                // Unpatch at runtime...
                // Now only disble a by if/return
            }
        }

        private static bool Check()
        {
            try
            {
                var mod = LoadedModManager.RunningMods.First(m => m.PackageId == "avius.locksdoorsexpanded");
                foundType = mod != null;
                Verse.Log.Message($"[PL] Trying to find: {mod}, Result: {foundType}");
            }
            catch
            {
                foundType = false;
            }
            return foundType;
        }
    }
}

