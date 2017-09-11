using System;
using System.IO;
using System.Xml.Linq;
using Verse;

namespace PrisonLabor
{
    public static class PrisonLaborPrefs
    {
        private static PrisonLaborPrefsData data;

        //TODO OLD DELETE WHEN BETA
        private static readonly string oldFilePath = Path.Combine(GenFilePaths.ConfigFolderPath, "PrisonData_Prefs.xml")
            ;

        private static readonly string prefsFilePath =
            Path.Combine(GenFilePaths.ConfigFolderPath, "PrisonLabor_Prefs.xml");

        public static Version Version
        {
            get { return data.version; }
            set
            {
                data.version = value;
                Apply();
            }
        }

        public static Version LastVersion
        {
            get { return data.last_version; }
            set
            {
                data.last_version = value;
                Apply();
            }
        }

        public static bool ShowNews
        {
            get { return data.show_news; }
            set
            {
                data.show_news = value;
                Apply();
            }
        }

        public static bool AllowAllWorkTypes
        {
            get { return data.allow_all_worktypes; }
            set
            {
                data.allow_all_worktypes = value;
                Apply();
            }
        }

        public static bool EnableMotivationMechanics
        {
            get
            {
                if (data.disable_mod)
                    return false;
                return data.enable_motivation_mechanics;
            }
            set
            {
                data.enable_motivation_mechanics = value;
                Apply();
            }
        }

        public static bool DisableMod
        {
            get { return data.disable_mod; }
            set
            {
                data.disable_mod = value;
                Apply();
            }
        }

        public static bool AdvancedGrowing
        {
            get { return data.advanced_growing; }
            set
            {
                data.advanced_growing = value;
                Apply();
            }
        }

        public static string AllowedWorkTypes
        {
            get { return data.allowed_works; }
            set
            {
                data.allowed_works = value;
                Apply();
            }
        }

        public static void Init()
        {
            // TODO delete after beta
            if (new FileInfo(oldFilePath).Exists)
                File.Move(oldFilePath, prefsFilePath);
            var flag = !new FileInfo(prefsFilePath).Exists;
            data = new PrisonLaborPrefsData();
            data = DirectXmlLoader.ItemFromXmlFile<PrisonLaborPrefsData>(prefsFilePath, true);
            Apply();
        }

        public static void Save()
        {
            Tutorials.UpdateTutorialFlags();
            try
            {
                var xDocument = new XDocument();
                var content = DirectXmlSaver.XElementFromObject(data, typeof(PrisonLaborPrefsData));
                xDocument.Add(content);
                xDocument.Save(prefsFilePath);
            }
            catch (Exception ex)
            {
                GenUI.ErrorDialog("ProblemSavingFile".Translate(prefsFilePath, ex.ToString()));
                Log.Error("Exception saving prefs: " + ex);
            }
        }

        public static void Apply()
        {
            data.Apply();
            PrisonLaborUtility.AllowedWorkTypesData = AllowedWorkTypes;
            Tutorials.Apply();
        }

        public static void RestoreToDefault()
        {
            var version = data.version;
            var last_version = data.last_version;
            var tutorials = data.tutorials_flags;

            data = new PrisonLaborPrefsData();

            data.version = version;
            data.last_version = last_version;
            data.tutorials_flags = tutorials;

            Apply();
        }

        public static void AddTutorialFlag(TutorialFlag flag)
        {
            data.tutorials_flags = data.tutorials_flags | flag;
        }

        public static bool HasTutorialFlag(TutorialFlag flag)
        {
            if ((data.tutorials_flags & flag) != 0)
                return true;
            return false;
        }

        public static void ResetTutorials()
        {
            data.tutorials_flags = TutorialFlag.None;
            Tutorials.Reset();
        }
    }
}