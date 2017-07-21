using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Verse;

namespace PrisonLabor
{
    public static class PrisonLaborPrefs
    {
        private static PrisonLaborPrefsData data;
        private static string prefsFilePath = Path.Combine(GenFilePaths.ConfigFolderPath, "PrisonData_Prefs.xml");

        public static int Version
        {
            get
            {
                return PrisonLaborPrefs.data.version;
            }
            set
            {
                PrisonLaborPrefs.data.version = value;
                PrisonLaborPrefs.Apply();
            }
        }

        public static int LastVersion
        {
            get
            {
                return PrisonLaborPrefs.data.last_version;
            }
            set
            {
                PrisonLaborPrefs.data.last_version = value;
                PrisonLaborPrefs.Apply();
            }
        }

        public static bool ShowNews
        {
            get
            {
                return PrisonLaborPrefs.data.show_news;
            }
            set
            {
                PrisonLaborPrefs.data.show_news = value;
                PrisonLaborPrefs.Apply();
            }
        }

        public static bool AllowAllWorktypes
        {
            get
            {
                return PrisonLaborPrefs.data.allow_all_worktypes;
            }
            set
            {
                PrisonLaborPrefs.data.allow_all_worktypes = value;
                PrisonLaborPrefs.Apply();
            }
        }

        public static bool EnableMotivationMechanics
        {
            get
            {
                return PrisonLaborPrefs.data.enable_motivation_mechanics;
            }
            set
            {
                PrisonLaborPrefs.data.enable_motivation_mechanics = value;
                PrisonLaborPrefs.Apply();
            }
        }

        public static bool DisableMod
        {
            get
            {
                return PrisonLaborPrefs.data.disable_mod;
            }
            set
            {
                PrisonLaborPrefs.data.disable_mod = value;
                PrisonLaborPrefs.Apply();
            }
        }

        public static void Init()
        {
            bool flag = !new FileInfo(prefsFilePath).Exists;
            PrisonLaborPrefs.data = new PrisonLaborPrefsData();
            PrisonLaborPrefs.data = DirectXmlLoader.ItemFromXmlFile<PrisonLaborPrefsData>(prefsFilePath, true);
            if (flag)
            {
                ;
            }
        }

        public static void Save()
        {
            try
            {
                XDocument xDocument = new XDocument();
                XElement content = DirectXmlSaver.XElementFromObject(PrisonLaborPrefs.data, typeof(PrisonLaborPrefsData));
                xDocument.Add(content);
                xDocument.Save(prefsFilePath);
            }
            catch (Exception ex)
            {
                GenUI.ErrorDialog("ProblemSavingFile".Translate(new object[]
                {
                    prefsFilePath,
                    ex.ToString()
                }));
                Log.Error("Exception saving prefs: " + ex);
            }
        }

        public static void Apply()
        {
            PrisonLaborPrefs.data.Apply();
        }
    }
}