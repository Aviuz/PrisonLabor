
using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
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
        private static bool foundMod;

        public static bool Found => foundMod;

        private static ModSearcher modSeeker;
        internal static void Init()
        {
            ModSearcher modSeeker = new ModSearcher("Locks");
            if (modSeeker.LookForMod())
            {
                foundMod = true;
            }
        }
    }
}

