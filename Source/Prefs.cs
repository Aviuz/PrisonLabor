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
        //OLD DELETE WHEN BETA
        private static string oldFilePath = Path.Combine(GenFilePaths.ConfigFolderPath, "PrisonData_Prefs.xml");
        private static string prefsFilePath = Path.Combine(GenFilePaths.ConfigFolderPath, "PrisonLabor_Prefs.xml");

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

        public static bool AllowAllWorkTypes
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
                if (data.disable_mod)
                    return false;
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

        public static bool AdvancedGrowing
        {
            get
            {
                return data.advanced_growing;
            }
            set
            {
                data.advanced_growing = value;
                Apply();
            }
        }

        public static string AllowedWorkTypes
        {
            get
            {
                return PrisonLaborPrefs.data.allowed_works;
            }
            set
            {
                PrisonLaborPrefs.data.allowed_works = value;
                PrisonLaborPrefs.Apply();
            }
        }

        public static void Init()
        {
            //delete after beta
            if (new FileInfo(oldFilePath).Exists)
            {
                System.IO.File.Move(oldFilePath, prefsFilePath);
            }
            bool flag = !new FileInfo(prefsFilePath).Exists;
            PrisonLaborPrefs.data = new PrisonLaborPrefsData();
            PrisonLaborPrefs.data = DirectXmlLoader.ItemFromXmlFile<PrisonLaborPrefsData>(prefsFilePath, true);
            Apply();
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
            data.Apply();
            PrisonLaborUtility.AllowedWorkTypesData = AllowedWorkTypes;
        }

        public static void RestoreToDefault()
        {
            int version = data.version;
            int last_version = data.last_version;
            data = new PrisonLaborPrefsData();
            data.version = version;
            data.last_version = last_version;
            Apply();
        }
    }
}