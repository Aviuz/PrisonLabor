using System;

namespace PrisonLabor.Core.Meta
{
    public class PrisonLaborPrefsData
    {
        public string defaultInteraction = "PrisonLabor_workOption";

        public string allowed_works = "";
        public bool allow_all_worktypes = false;
        public bool advanced_growing = false;
        public bool enable_motivation_mechanics = true;
        public bool enable_motivation_icons = true;
        public bool enable_revolts = true;
        public bool show_treatment_happiness = false;
        public bool enable_suicide = true;
        public bool enable_full_heal_rest = true;

        public Version last_version = Version.v0_0;
        public bool show_news = true;
        public bool enable_reports = false;

        public TutorialFlag tutorials_flags = TutorialFlag.None;
        public Version version = Version.v0_0;

        public void Apply()
        {
        }
    }

    [Flags]
    public enum TutorialFlag
    {
        None = 0x0,
        Introduction = 0x1,
        Motivation = 0x2,
        Growing = 0x4,
        Managment = 0x8,
        Timetable = 0x10,
        LaborAreaWarning = 0x20,
        Treatment = 0x40,
    }
}