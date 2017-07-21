using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrisonLabor
{
    public class PrisonLaborPrefsData
    {
        public int version = -1;
        public int last_version = -1;
        public bool show_news = true;
        public bool allow_all_worktypes = false;
        public bool enable_motivation_mechanics = true;
        public bool disable_mod = false;

        public PrisonLaborPrefsData()
        {
            
        }

        public void Apply()
        {
        }
    }
}
