using PrisonLabor.Core.LaborWorkSettings;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using System;
using System.IO;
using System.Xml.Linq;
using Verse;

namespace PrisonLabor.Core.Meta
{
    public static class PrisonLaborPrefs
    {
        private static PrisonLaborPrefsData data;

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

        public static string DefaultInteractionMode
        {
            get
            {
                return data.defaultInteraction;
            }

            set
            {
                data.defaultInteraction = value;
                Apply();
            }
        }

        public static bool EnableMotivationMechanics
        {
            get
            {
                return data.enable_motivation_mechanics;
            }
            set
            {
                data.enable_motivation_mechanics = value;
                Apply();
            }
        }

        public static bool EnableMotivationIcons
        {
            get
            {
                return data.enable_motivation_icons;
            }
            set
            {
                data.enable_motivation_icons = value;
                Apply();
            }
        }

        public static bool EnableRevolts
        {
            get
            {
                return data.enable_revolts;
            }
            set
            {
                data.enable_revolts = value;
                Apply();
            }
        }

        public static bool ShowTreatmentHappiness
        {
            get
            {
                return data.show_treatment_happiness;
            }
            set
            {
                data.show_treatment_happiness = value;
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
            var flag = !new FileInfo(prefsFilePath).Exists;
            data = new PrisonLaborPrefsData();
            data = DirectXmlLoader.ItemFromXmlFile<PrisonLaborPrefsData>(prefsFilePath, true);
            Apply();
        }

        public static void Save()
        {
            Other.Tutorials.UpdateTutorialFlags();
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
            WorkSettings.DataString = AllowedWorkTypes;
            Tutorials.Apply();
            Need_Treatment.ShowOnList = ShowTreatmentHappiness;
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