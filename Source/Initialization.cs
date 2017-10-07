using PrisonLabor.HarmonyPatches;
using PrisonLabor.Tweaks;
using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    internal class Initialization
    {
        static Initialization()
        {
            HPatcher.Init();
            ClassInjector.Init();
            PrisonLaborPrefs.Init();
            PrisonLaborMod.Init();
            VersionUtility.CheckVersion();
            Designator_AreaLabor.Initialization();
            Behaviour_MotivationIcon.Initialization();
            CompatibilityPatches.Initialization.Run();
            HediffManager.Init();
        }
    }
}