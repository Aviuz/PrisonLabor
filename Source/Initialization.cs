using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    internal class Initialization
    {
        static Initialization()
        {
            HarmonyPatches.HarmonyPatches.Init();
            PrisonLaborPrefs.Init();
            PrisonLaborMod.Init();
            VersionUtility.CheckVersion();
            Designator_AreaLabor.Initialization();
            Behaviour_MotivationIcon.Initialization();
            CompatibilityPatches.Initialization.Run();
            HediffGiver_PrisonersChains.Init();
        }
    }
}