using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrisonLabor
{
    public class PrisonLaborPrefsData
    {
        public Version version = Version.v0_0;
        public Version last_version = Version.v0_0;
        public bool show_news = true;
        public bool allow_all_worktypes = false;
        public bool enable_motivation_mechanics = true;
        public bool disable_mod = false;
        public bool advanced_growing = false;

        public string allowed_works = "";

        public TutorialFlag tutorials_flags = TutorialFlag.None;

        public PrisonLaborPrefsData()
        {

        }

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
    }

    [Flags]
    public enum TutorialFlag
    {
        None = 0x0,
        Introduction = 0x1,
        Motivation = 0x2,
        Growing = 0x4,
        Managment = 0x8,
        Timetable = 0x10
    }
}
