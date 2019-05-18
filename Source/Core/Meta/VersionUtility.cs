using PrisonLabor.Core.Windows;

namespace PrisonLabor.Core.Meta
{
    class VersionUtility
    {
        public const Version versionNumber = Version.v0_10_0;
        public const string versionString = "0.10.0 [DEV]";

        public static Version VersionOfSaveFile { get; set; }

        public static void CheckVersion()
        {
            // Update actual version
            if (PrisonLaborPrefs.Version == Version.v0_0)
            {
                PrisonLaborPrefs.Version = versionNumber;
                PrisonLaborPrefs.LastVersion = versionNumber;
            }
            else if (PrisonLaborPrefs.Version != versionNumber)
            {
                PrisonLaborPrefs.Version = versionNumber;
            }

            // Client has new version
            if (PrisonLaborPrefs.LastVersion < versionNumber)
            {
                // Show version news
                NewsWindow.LastVersionString = GetVersionString(PrisonLaborPrefs.LastVersion);
                NewsWindow.AutoShow = true;

                // Pre 0.9.4
                if (PrisonLaborPrefs.LastVersion < Version.v0_9_4)
                {
                    CompatibilityPatches.OlderVersions.Pre_v0_9_4();
                }
            }

            PrisonLaborPrefs.Version = versionNumber;
            PrisonLaborPrefs.Save();
        }

        public static string GetVersionString(Version version)
        {
            return version.ToString().Replace("_", ".").Substring(1);
        }
    }
}
