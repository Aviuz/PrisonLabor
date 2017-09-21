using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    internal class Initialization
    {
        static Initialization()
        {
            HarmonyPatches.Initialization.Run();
            PrisonLaborPrefs.Init();
            PrisonLaborMod.Init();
            checkVersion();
            Designator_AreaLabor.Initialization();
            Behaviour_MotivationIcon.Initialization();
            CompatibilityPatches.Initialization.Run();
        }

        private static void checkVersion()
        {
            // Update actual version
            if (PrisonLaborPrefs.Version == Version.v0_0)
            {
                PrisonLaborPrefs.Version = PrisonLaborMod.versionNumber;
                PrisonLaborPrefs.LastVersion = PrisonLaborMod.versionNumber;
            }
            else if (PrisonLaborPrefs.Version != PrisonLaborMod.versionNumber)
            {
                PrisonLaborPrefs.Version = PrisonLaborMod.versionNumber;
            }

            // Check for news
            if (PrisonLaborPrefs.LastVersion < Version.v0_5)
            {
                NewsDialog.news_0_5 = true;
                NewsDialog.autoShow = true;
            }
            if (PrisonLaborPrefs.LastVersion < Version.v0_6)
            {
                NewsDialog.news_0_6 = true;
                NewsDialog.autoShow = true;
            }
            if (PrisonLaborPrefs.LastVersion < Version.v0_7)
            {
                NewsDialog.news_0_7 = true;
                NewsDialog.autoShow = true;
            }
            //TODO delete dev version on full-release
            if (PrisonLaborPrefs.LastVersion < Version.v0_7_dev2)
            {
                NewsDialog.news_0_7_dev2 = true;
                NewsDialog.autoShow = true;
            }
            if (PrisonLaborPrefs.LastVersion < Version.v0_7_dev3)
            {
                NewsDialog.news_0_7_dev3 = true;
                NewsDialog.autoShow = true;
            }

            Log.Message($"Enabled Prison Labor v{PrisonLaborMod.versionString}");
            PrisonLaborPrefs.Version = PrisonLaborMod.versionNumber;
            PrisonLaborPrefs.Save();
        }
    }
}