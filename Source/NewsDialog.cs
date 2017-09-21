using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    internal class NewsDialog : Window
    {
        // Constans
        private const float Spacing = 2f;
        private const float GapHeight = 12f;
        private const string MarginText = " - ";
        private readonly float MarginWidth = Text.fontStyles[1].CalcSize(new GUIContent(MarginText)).x;
        private const GameFont TitleFont = GameFont.Medium;
        private const GameFont ItemFont = GameFont.Small;

        // Static Properties
        public static bool autoShow;

        public static bool showAll = false;

        public static bool news_0_5 = false;
        public static bool news_0_6 = false;
        public static bool news_0_7 = false;
        //TODO delete dev versions (news only, do not delete from enum!!!!)
        public static bool news_0_7_dev2 = true;
        public static bool news_0_7_dev3 = true;

        // Fields
        private string[] titles;
        private string[][] items;

        private Vector2 position;

        public NewsDialog()
        {
            doCloseButton = true;
            doCloseX = true;
            Init();
        }

        public void Init()
        {
            List<string> titlesList = new List<string>();
            List<string[]> itemsList = new List<string[]>();

            if (news_0_7_dev3 || showAll)
            {
                titlesList.Add("Prison Labor Alpha v0.7 dev3");
                string[] itemsArray =
                {
                    "Fixed SeedsPlease + Forbidden bug",
                    "Rewritten news dialog",
                    "Added \"Work and recruit\" option",
                };
                itemsList.Add(itemsArray);
            }
            if (news_0_7_dev2 || showAll)
            {
                titlesList.Add("Prison Labor Alpha v0.7 dev2");
                string[] itemsArray =
                {
                    "Added Seeds Please compatibilty (but enables the forbidden bug when harvesting crops, when SP mod is activated - TODO)",
                    "Changed insiration mechanics, now prisoners will run after shor delay if left unwatched",
                    "Added watching prisoners that can escape (and will after delay) to supervise job",
                };
                itemsList.Add(itemsArray);
            }
            if (news_0_7 || showAll)
            {
                titlesList.Add("Prison Labor Alpha v0.7");
                string[] itemsArray =
                {
                    "Added settings! You can now change almost any aspect of this mod, including:\n   * work types\n   * motivation mechanics\n   * prevention of planting advanced plants.",
                    "Added \"uninstaller\" (\"disable\" option in settings), which will allow to disable this mod from existing saves.",
                    "\"No more beeping!\". Changed way of informing player what's going on with prisoners. It should be less annoying and more insightful.",
                    "Fixed bugs, including bug that prevents prisoners from cleaning and bug that causes warden to stuck in loop of delivering food to prisoner.",
                    "\"No more watching while prisoner is sleeping.\"Wardens will no longer watch over not working prisoners.",
                    "Prisoners will now stay in bed while waiting for operation",
                    "Prisoners will now stop work when starving for default (\"Anything\" time), instead of hungry. They will still get minor debuff.",
                };
                itemsList.Add(itemsArray);
            }
            if (news_0_6 || showAll)
            {
                titlesList.Add("Prison Labor Alpha v0.6");
                string[] itemsArray =
                {
                    "Time restrictions - now you can manage your prisoners time for sleep, work and joy. You can now even force them to work when they're hungry!",
                    "Getting food by prisoners - Now prisoners will look for food in much better way, and now (when they desperate enough) they will eat corpses!",
                    "\"Laziness\" changed to \"Motivation\" and inverted.\n\n   ATTENTION: After PrisonLabor reaches beta all saves with PrisonLabor v0.5a or lower will be corrupted and unplayable. This version (0.6) is safe and converts all older saves.",
                };
                itemsList.Add(itemsArray);
            }
            if (news_0_5 || showAll)
            {
                titlesList.Add("Prison Labor Alpha v0.5");
                string[] itemsArray =
                {
                    "Prisoners can now grow, but only plants that not require any skills.",
                    "You can now manage prisoners work types. Just check \"Work\" tab!",
                    "Laziness now appear on \"Needs\" tab. Above 50% wardens will watch prisoners. Above 80% prisoners won't work unless supervised.",
                    "Wardens will now bring food to prisoners that went too far from his bed.",
                    "Prisoners won't gain laziness when not working anymore.",
                    "Fixed many bugs",
                };
                itemsList.Add(itemsArray);
            }

            // If count of items in both lists aren't equal that means someone (me) fucked up
            if (titlesList.Count != itemsList.Count)
                throw new System.Exception("Prison Labor exception: news lists aren't equal");

            // Transfer items: dynamic list => static array, for optimalization  
            titles = new string[titlesList.Count];
            for (int i = 0; i < titlesList.Count; i++)
            {
                titles[i] = titlesList[i];
            }
            items = new string[itemsList.Count][];
            for (int i = 0; i < itemsList.Count; i++)
            {
                items[i] = itemsList[i];
            }
        }

        public static void TryShow()
        {
            if (autoShow && PrisonLaborPrefs.ShowNews)
            {
                Find.WindowStack.Add(new NewsDialog());
                PrisonLaborPrefs.LastVersion = PrisonLaborPrefs.Version;
                PrisonLaborPrefs.Save();
                autoShow = false;
            }
        }

        public static void ForceShow()
        {
            Find.WindowStack.Add(new NewsDialog());
            PrisonLaborPrefs.LastVersion = PrisonLaborPrefs.Version;
            PrisonLaborPrefs.Save();
            autoShow = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var displayRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 50f);
            var viewRect = new Rect(0, 0, inRect.width - 16f, CalculateHeight(inRect.width - 16f));

            Widgets.BeginScrollView(displayRect, ref position, viewRect, true);

            for (int i = 0; i < titles.Length; i++)
            {
                // Draw title
                Text.Font = TitleFont;
                Widgets.Label(viewRect, titles[i]);
                viewRect.y += Text.CalcHeight(titles[i], viewRect.width) + Spacing;

                // Draw line gap
                Color color = GUI.color;
                GUI.color = GUI.color * new Color(1f, 1f, 1f, 0.4f);
                Widgets.DrawLineHorizontal(viewRect.x, viewRect.y + +GapHeight * 0.5f, viewRect.width);
                GUI.color = color;
                viewRect.y += GapHeight;

                // Draw items
                Text.Font = ItemFont;
                viewRect.width -= MarginWidth;
                for (int j = 0; j < items[i].Length; j++)
                {
                    Widgets.Label(viewRect, MarginText);
                    viewRect.x += MarginWidth;
                    Widgets.Label(viewRect, items[i][j]);
                    viewRect.x -= MarginWidth;
                    viewRect.y += Text.CalcHeight(items[i][j], viewRect.width) + Spacing;
                }
                viewRect.width += MarginWidth;

                // Make gap
                viewRect.y += GapHeight;
            }

            Widgets.EndScrollView();
        }

        private float CalculateHeight(float width)
        {
            float height = 0;
            foreach (var item in titles)
                height += Text.CalcHeight(item, width) + Spacing + GapHeight;
            foreach (var array in items)
                foreach (var item in array)
                    height += Text.CalcHeight(item, width - MarginWidth) + Spacing;
            return height;
        }
    }
}