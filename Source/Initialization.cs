using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    internal class Initialization
    {
        public static Version version = PrisonLaborMod.versionNumber;

        static Initialization()
        {
            HarmonyPatches.Initialization.Run();
            PrisonLaborPrefs.Init();
            PrisonLaborMod.Init();
            checkVersion();
            Designator_AreaLabor.Initialization();
            Behaviour_MotivationIcon.Initialization();
        }

        private static void checkVersion()
        {
            //delete later
            if (PrisonLaborPrefs.Version > Version.v0_2 && PrisonLaborPrefs.Version < Version.v0_6)
                PrisonLaborPrefs.LastVersion = PrisonLaborPrefs.Version;

            // Update actual version
            if (PrisonLaborPrefs.Version == Version.v0_0)
            {
                PrisonLaborPrefs.Version = version;
                PrisonLaborPrefs.LastVersion = version;
            }
            else if (PrisonLaborPrefs.Version != version)
            {
                PrisonLaborPrefs.Version = version;
            }

            // Check for news
            if (PrisonLaborPrefs.LastVersion < Version.v0_5)
            {
                Log.Message("Detected older version of PrisonLabor than 0.5");
                NewsDialog.news_0_5 = true;
                NewsDialog.autoShow = true;
            }
            if (PrisonLaborPrefs.LastVersion < Version.v0_6)
            {
                Log.Message("Detected older version of PrisonLabor than 0.6");
                NewsDialog.news_0_6 = true;
                NewsDialog.autoShow = true;
            }
            if (PrisonLaborPrefs.LastVersion < Version.v0_7)
            {
                Log.Message("Detected older version of PrisonLabor than 0.7");
                NewsDialog.news_0_7 = true;
                NewsDialog.autoShow = true;
            }

            Log.Message("Loaded PrisonLabor v" + PrisonLaborPrefs.Version);
            PrisonLaborPrefs.Version = version;
            PrisonLaborPrefs.Save();
        }
    }
}