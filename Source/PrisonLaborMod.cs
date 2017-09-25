using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    internal class PrisonLaborMod : Mod
    {
        private static string difficulty = "";
        private static bool showNews;
        private static bool allowAllWorktypes;
        private static bool enableMotivationMechanics;
        private static bool advanceGrowing;
        private static bool disableMod;
        private static int defaultInteractionMode;

        private static List<PrisonerInteractionModeDef> interactionModeList;

        public PrisonLaborMod(ModContentPack content) : base(content)
        {
        }

        public static void Init()
        {
            showNews = PrisonLaborPrefs.ShowNews;
            allowAllWorktypes = PrisonLaborPrefs.AllowAllWorkTypes;
            enableMotivationMechanics = PrisonLaborPrefs.EnableMotivationMechanics;
            disableMod = PrisonLaborPrefs.DisableMod;

            interactionModeList = new List<PrisonerInteractionModeDef>(DefDatabase<PrisonerInteractionModeDef>.AllDefs);
            defaultInteractionMode = interactionModeList.IndexOf(DefDatabase<PrisonerInteractionModeDef>.GetNamed(PrisonLaborPrefs.DefaultInteractionMode));
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var leftRect = new Rect(inRect.x, inRect.y, inRect.width * 0.65f, inRect.height);
            var rightRect = new Rect(inRect.x + inRect.width * 0.65f + 30f, inRect.y, inRect.width * 0.35f - 30f,
                inRect.height);

            var listing_options = new Listing_Standard();

            listing_options.Begin(leftRect);

            listing_options.CheckboxLabeled("Show news", ref showNews,
                "Showing news about changes in mod when prisoners detected.");

            listing_options.GapLine();

            if (listing_options.ButtonTextLabeled("Default prisoner interaction mode", PrisonerInteractionModeUtility.GetLabel(interactionModeList[defaultInteractionMode])))
                defaultInteractionMode = defaultInteractionMode < interactionModeList.Count - 1 ? defaultInteractionMode + 1 : 0;

            listing_options.GapLine();

            if (!disableMod)
            {
                listing_options.Label("Allowed work types:", -1f);
                listing_options.CheckboxLabeled("   allow all", ref allowAllWorktypes, "allow all work types");
                if (!allowAllWorktypes)
                {
                    if (listing_options.ButtonTextLabeled("   allowed work types:", "browse"))
                        Find.WindowStack.Add(new SelectWorkTypesDialog());
                }
                else
                {
                    listing_options.Gap();
                }

                listing_options.GapLine();

                listing_options.CheckboxLabeled("Motivation mechanics (!)", ref enableMotivationMechanics,
                    "When checked prisoners need to be motivated.\n\nWARINING: Needs reloading save.");

                listing_options.GapLine();

                listing_options.CheckboxLabeled("Prisoners can grow advanced plants", ref advanceGrowing,
                    "When disabled prisoners can only grow plants that not require any skills.");
            }
            else
            {
                listing_options.Gap();
                listing_options.Gap();
                listing_options.Label("Restart then re-save your game.", -1f);
                listing_options.Label("After this steps you can safely disable this mod.", -1f);
                listing_options.Gap();
                listing_options.Gap();
                listing_options.Gap();
            }

            listing_options.Gap();
            listing_options.Gap();
            listing_options.Gap();

            listing_options.CheckboxLabeled("Disable mod", ref disableMod,
                "When enabled, worlds that are saved are transferred to 'safe Mode', and can be played without mod.");

            listing_options.End();

            var listing_panel = new Listing_Standard();

            listing_panel.Begin(rightRect);

            var heigh_temp = rightRect.width * 0.56f;
            GUI.DrawTexture(new Rect(0, 0, rightRect.width, heigh_temp), ContentFinder<Texture2D>.Get("Preview", true));
            listing_panel.Gap(heigh_temp);
            listing_panel.Label("Prison Labor Alpha", -1f);
            listing_panel.Label("Version: " + VersionUtility.versionString, -1f);

            listing_panel.GapLine();

            listing_panel.Label("Difficulty: " + difficulty, -1f);

            listing_panel.GapLine();

            if (listing_panel.ButtonText("Defaults"))
            {
                PrisonLaborPrefs.RestoreToDefault();
                Init();
            }

            if (listing_panel.ButtonText("ShowNews"))
            {
                NewsDialog.showAll = true;
                NewsDialog.ForceShow();
            }

            listing_panel.End();

            Apply();
        }

        public override string SettingsCategory()
        {
            return "Prison Labor";
        }

        public override void WriteSettings()
        {
            Log.Message("saved");
            PrisonLaborPrefs.ShowNews = showNews;
            PrisonLaborPrefs.AllowAllWorkTypes = allowAllWorktypes;
            if (!disableMod)
                PrisonLaborPrefs.EnableMotivationMechanics = enableMotivationMechanics;
            PrisonLaborPrefs.AdvancedGrowing = advanceGrowing;
            PrisonLaborPrefs.DisableMod = disableMod;
            PrisonLaborPrefs.DefaultInteractionMode = interactionModeList[defaultInteractionMode].defName;
            PrisonLaborPrefs.Save();
        }

        private static void Apply()
        {
            PrisonLaborPrefs.Apply();
            CalculateDifficulty();
        }

        private static void CalculateDifficulty()
        {
            var value = 1000;
            if (!enableMotivationMechanics)
                value -= 300;
            if (advanceGrowing)
                value -= 50;
            value -= 500;
            if (!allowAllWorktypes)
            {
                var delta = 500 + 7 * 50 + (DefDatabase<WorkTypeDef>.DefCount - 20) * 25;
                foreach (var wtd in DefDatabase<WorkTypeDef>.AllDefs)
                    if (!PrisonLaborUtility.WorkDisabled(wtd))
                        delta -= 50;
                if (delta > 0)
                    value += delta;
            }

            if (value >= 1000)
                difficulty = value / 10 + " (Normal)";
            else if (value >= 800)
                difficulty = value / 10 + " (Casual)";
            else if (value >= 500)
                difficulty = value / 10 + " (Easy)";
            else if (value >= 300)
                difficulty = value / 10 + " (Peaceful)";
            else
                difficulty = value / 10 + " (A joke)";
        }
    }
}