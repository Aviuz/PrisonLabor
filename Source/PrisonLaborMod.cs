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
        private static bool enableMotivationIcons;
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
            enableMotivationIcons = PrisonLaborPrefs.EnableMotivationIcons;
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

            listing_options.CheckboxLabeled("PrisonLabor_ShowNews".Translate(), ref showNews,
                "PrisonLabor_ShowNewsDesc".Translate());

            listing_options.GapLine();

            if (listing_options.ButtonTextLabeled("PrisonLabor_DefaultInterMode".Translate(), interactionModeList[defaultInteractionMode].LabelCap))
                defaultInteractionMode = defaultInteractionMode < interactionModeList.Count - 1 ? defaultInteractionMode + 1 : 0;

            listing_options.GapLine();

            if (!disableMod)
            {
                listing_options.Label("PrisonLabor_AllowedWorkTypes".Translate(), -1f);
                listing_options.CheckboxLabeled("   " + "PrisonLabor_AllowAll".Translate(), ref allowAllWorktypes, "PrisonLabor_AllowAllWorkTypes".Translate());
                if (!allowAllWorktypes)
                {
                    if (listing_options.ButtonTextLabeled("   " + "PrisonLabor_AllowedWorkTypesL".Translate(), "PrisonLabor_Browse".Translate()))
                        Find.WindowStack.Add(new SelectWorkTypesDialog());
                }
                else
                {
                    listing_options.Gap();
                }

                listing_options.GapLine();

                listing_options.CheckboxLabeled("PrisonLabor_MotivationMechanics".Translate(), ref enableMotivationMechanics,
                    "PrisonLabor_MotivationWarning".Translate());

                listing_options.GapLine();

                listing_options.CheckboxLabeled("PrisonLabor_MotivationIcons".Translate(), ref enableMotivationIcons,
                    "PrisonLabor_MotivationIconsDesc".Translate());

                listing_options.GapLine();

                listing_options.CheckboxLabeled("PrisonLabor_CanGrowAdvanced".Translate(), ref advanceGrowing,
                    "PrisonLabor_CanGrowAdvancedDesc".Translate());
            }
            else
            {
                listing_options.Gap();
                listing_options.Gap();
                listing_options.Label("PrisonLabor_RestartInfo".Translate(), -1f);
                listing_options.Label("PrisonLabor_RestartInfo2".Translate(), -1f);
                listing_options.Gap();
                listing_options.Gap();
                listing_options.Gap();
            }

            listing_options.Gap();
            listing_options.Gap();
            listing_options.Gap();

            listing_options.CheckboxLabeled("PrisonLabor_DisableMod".Translate(), ref disableMod,
                "PrisonLabor_DisableModDesc".Translate());

            listing_options.End();

            var listing_panel = new Listing_Standard();

            listing_panel.Begin(rightRect);

            var heigh_temp = rightRect.width * 0.56f;
            GUI.DrawTexture(new Rect(0, 0, rightRect.width, heigh_temp), ContentFinder<Texture2D>.Get("Preview", true));
            listing_panel.Gap(heigh_temp);
            listing_panel.Label("Prison Labor", -1f);
            listing_panel.Label("PrisonLabor_Version".Translate() + VersionUtility.versionString, -1f);

            listing_panel.GapLine();

            listing_panel.Label("PrisonLabor_Difficulty".Translate() + difficulty, -1f);

            listing_panel.GapLine();

            if (listing_panel.ButtonText("PrisonLabor_Defaults".Translate()))
            {
                PrisonLaborPrefs.RestoreToDefault();
                Init();
            }

            if (listing_panel.ButtonText("PrisonLabor_ShowNews".Translate()))
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
            PrisonLaborPrefs.ShowNews = showNews;
            PrisonLaborPrefs.AllowAllWorkTypes = allowAllWorktypes;
            if (!disableMod)
                PrisonLaborPrefs.EnableMotivationMechanics = enableMotivationMechanics;
            PrisonLaborPrefs.EnableMotivationIcons = enableMotivationIcons;
            PrisonLaborPrefs.AdvancedGrowing = advanceGrowing;
            PrisonLaborPrefs.DisableMod = disableMod;
            PrisonLaborPrefs.DefaultInteractionMode = interactionModeList[defaultInteractionMode].defName;
            PrisonLaborPrefs.Save();
            Log.Message("Prison Labor settings saved");
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
                difficulty = value / 10 + " (" + "PrisonLabor_DifficultyNormal".Translate() + ")";
            else if (value >= 800)
                difficulty = value / 10 + " (" + "PrisonLabor_DifficultyCasual".Translate() + ")";
            else if (value >= 500)
                difficulty = value / 10 + " (" + "PrisonLabor_DifficultyEasy".Translate() + ")";
            else if (value >= 300)
                difficulty = value / 10 + " (" + "PrisonLabor_DifficultyPeaceful".Translate() + ")";
            else
                difficulty = value / 10 + " (" + "PrisonLabor_DifficultyJoke".Translate() + ")";
        }
    }
}