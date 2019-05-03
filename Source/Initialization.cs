using PrisonLabor.Core.Hediffs;
using PrisonLabor.Core.LaborArea;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Settings;
using PrisonLabor.HarmonyPatches;
using PrisonLabor.Tweaks;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    internal class Initialization
    {
        static Initialization()
        {
            try
            {
                PrisonLaborPrefs.Init();
                HPatcher.Init();
                ClassInjector.Init();
                SettingsMenu.Init();
                VersionUtility.CheckVersion();
                Designator_AreaLabor.Initialization();
                CompatibilityPatches.Initialization.Run();
                HediffManager.Init();

                Log.Message($"Enabled Prison Labor v{VersionUtility.versionString}");
            }
            catch (Exception e)
            {
                Log.Error($"Prison Labor v{VersionUtility.versionString} caught error during start up:\n{e.ToString()}");
            }

        }
    }
}