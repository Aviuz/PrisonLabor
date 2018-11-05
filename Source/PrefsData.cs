using System;

namespace PrisonLabor
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

        public bool disable_mod = false;

        public Version last_version = Version.v0_0;
        public bool show_news = true;
        public bool enable_reports = false;

        public TutorialFlag tutorials_flags = TutorialFlag.None;
        public Version version = Version.v0_0;

        public void Apply()
        {
        }
    }

    public enum Version
    {
        v0_0,
        v0_1,
        v0_2,
        v0_3,
        v0_4,
        v0_5,
        v0_6,
        v0_7,
        v0_7_dev1,
        v0_7_dev2,
        v0_7_dev3,
        v0_7_dev4,
        v0_7_dev5,
        v0_8_0,
        v0_8_1,
        v0_8_2,
        v0_8_3,
        v0_8_4,
        v0_8_5,
        v0_8_6,
        v0_8_7,
        v0_8_8,
        v0_8_9,
        v0_8_9_1,
        v0_8_9_2,
        v0_8_9_4,
        v0_8_9_5,
        v0_9_0,
        v0_9_1,
        v0_9_2,
        v0_9_3,
        v0_9_4,
        v0_9_5,
        v0_9_6,
        v0_9_8,
        v0_9_9,
        v0_9_10,
        v0_9_11
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
    }
}