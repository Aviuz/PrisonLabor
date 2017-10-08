using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    class VersionUtility
    {
        public const Version versionNumber = Version.v0_8_8;
        public const string versionString = "0.8.8";

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
            if (PrisonLaborPrefs.LastVersion < Version.v0_8_0)
            {
                NewsDialog.news_0_8_0 = true;
                NewsDialog.autoShow = true;
            }
            if (PrisonLaborPrefs.LastVersion < Version.v0_8_1)
            {
                NewsDialog.news_0_8_1 = true;
                NewsDialog.autoShow = true;
            }
            if (PrisonLaborPrefs.LastVersion < Version.v0_8_3)
            {
                NewsDialog.news_0_8_3 = true;
                NewsDialog.autoShow = true;
            }
            if (PrisonLaborPrefs.LastVersion < Version.v0_8_6)
            {
                NewsDialog.news_0_8_6 = true;
                NewsDialog.autoShow = true;
            }

            Log.Message($"Enabled Prison Labor v{versionString}");
            PrisonLaborPrefs.Version = versionNumber;
            PrisonLaborPrefs.Save();
        }
    }
}
