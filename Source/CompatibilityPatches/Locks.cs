
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
    internal static class Locks
    {
        public static bool foundType;
        internal static void Init()
        {
            if (Check())
            {
                Patch();
            }
        }

        private static void Patch()
        {

        }

        private static bool Check()
        {
            try
            {
                var mod = LoadedModManager.RunningMods.First(m => m.PackageId == "avius.locks");
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

