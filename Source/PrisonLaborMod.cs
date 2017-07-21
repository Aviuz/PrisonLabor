using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    class PrisonLaborMod : Mod
    {
        private static bool showNews;
        private static bool allowAllWorktypes;
        private static bool enableMotivationMechanics;
        private static bool disableMod;

        public PrisonLaborMod(ModContentPack content) : base(content)
        {
        }

        public static void Init()
        {
            showNews = PrisonLaborPrefs.ShowNews;
            allowAllWorktypes = PrisonLaborPrefs.AllowAllWorktypes;
            enableMotivationMechanics = PrisonLaborPrefs.EnableMotivationMechanics;
            disableMod = PrisonLaborPrefs.DisableMod;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();

            listing_Standard.Begin(inRect);
            listing_Standard.Label("Version: " + PrisonLaborPrefs.Version, -1f);

            listing_Standard.GapLine();

            listing_Standard.CheckboxLabeled("Show news", ref showNews, "Show news");

            listing_Standard.GapLine();

            listing_Standard.Label("Allowed work types:", -1f);
            listing_Standard.CheckboxLabeled("   allow all", ref allowAllWorktypes, "allow all");
            if (!allowAllWorktypes)
            {
                if (listing_Standard.ButtonTextLabeled("   enabled:", "select"))
                    ;//add dialog
            }
            else
            {
                listing_Standard.Gap();
            }

            listing_Standard.GapLine();

            listing_Standard.CheckboxLabeled("Motivation mechanics", ref enableMotivationMechanics, "When checked prisoners need to be motivated.");

            listing_Standard.GapLine();

            listing_Standard.CheckboxLabeled("Disable", ref disableMod, "When enabled, worlds that are saved are transferred to 'safe mode', and can be played without mod.");

            listing_Standard.End();
            Save();
        }

        public override string SettingsCategory()
        {
            return "Prison Labor";
        }

        public void Save()
        {
            if (
                PrisonLaborPrefs.ShowNews != showNews &&
                PrisonLaborPrefs.AllowAllWorktypes != allowAllWorktypes &&
                PrisonLaborPrefs.EnableMotivationMechanics != enableMotivationMechanics &&
                PrisonLaborPrefs.DisableMod != disableMod
            )
            {
                PrisonLaborPrefs.ShowNews = showNews;
                PrisonLaborPrefs.AllowAllWorktypes = allowAllWorktypes;
                PrisonLaborPrefs.EnableMotivationMechanics = enableMotivationMechanics;
                PrisonLaborPrefs.DisableMod = disableMod;
                PrisonLaborPrefs.Save();
            }
        }
    }
}
