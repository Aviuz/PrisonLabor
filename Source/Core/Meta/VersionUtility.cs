using PrisonLabor.Core.Other;
using PrisonLabor.Core.Windows;

namespace PrisonLabor.Core.Meta
{
    public class VersionUtility
    {
        public const Version versionNumber = Version.v1_4_0;
        public const string versionString = "1.4.0";

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
                NewsWindow.AutoShow = NewsProvider.ShouldAutoShowChangelog(NewsWindow.LastVersionString);

                // Dev version fix, it can be removed in future
                // There is no changelog for 0.10 so it will skip it, and display all changes
                if (PrisonLaborPrefs.LastVersion == Version.v0_10_0)
                {
                    NewsWindow.LastVersionString = GetVersionString(Version.v0_9_11);
                }

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
