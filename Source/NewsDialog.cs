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
        public static bool news_0_8_0 = false;
        public static bool news_0_8_1 = false;
        public static bool news_0_8_3 = false;
        public static bool news_0_8_6 = false;
        public static bool news_0_9_0 = false;
        public static bool news_0_9_1 = false;
        public static bool news_0_9_2 = false;

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

            // How to insert news:
            // [subtitle] for subtitle
            // [img] ... [/img] for image (inside name of file)
            // [gap] for gap

            // 0.9.2
            if (news_0_9_2 || showAll)
            {
                titlesList.Add("Prison Labor Beta v0.9.2");
                string[] itemsArray =
                {
                    "fixed seeds please compatibility issue",
                };
                itemsList.Add(itemsArray);
            }
            // 0.9.1
            if (news_0_9_1 || showAll)
            {
                titlesList.Add("Prison Labor Beta v0.9.1");
                string[] itemsArray =
                {
                    "changed max. skill required for non-advanced growing by prisoners to 6 instead of 0",
                    "added new work type Jailor",
                    "fixed drawing icons on world map",
                    "fixed disabling mod from existing saves",
                    "fixed incorrectly showing \"advanced growing by prisoners\" option",
                };
                itemsList.Add(itemsArray);
            }
            // 0.9.0
            if (news_0_9_0 || showAll)
            {
                titlesList.Add("Prison Labor Beta v0.9.0");
                string[] itemsArray =
                {
                    "updated to RimWorld beta v18",
                    "added option to disable icons above prisoners heads in mod menu",
                    "fixed error \"null reference in onGui()\" when loading save",
                };
                itemsList.Add(itemsArray);
            }
            // 0.8.8 (silent)
            if (showAll)
            {
                titlesList.Add("Prison Labor Beta v0.8.8");
                string[] itemsArray =
                {
                    "changed slow from prisoners chains to act as factor instead offset",
                    "fixed compatibility issues with Seeds Please(again)",
                };
                itemsList.Add(itemsArray);
            }
            // 0.8.7 (silent)
            if (showAll)
            {
                titlesList.Add("Prison Labor Beta v0.8.7");
                string[] itemsArray =
                {
                    "fixed bug with dropping motivation while in bed",
                    "prisoners will now get different weapons when revolt triggers (molotovs, bows, or clubs)",
                    "replaced orginal jobs with \"tweak\" jobs (instead of overriding them, this fix is for users who use \"WorkTab\" by Fluffy)",
                    "removed warning message from logs",
                    "prisoners will now have 50% of normal speed in chains (instead of 35%)",
                    "prisoners will now break chains after some period of time instead of immadiately(matter in incidents, breakouts etc.)",
                    "wardens will now try to motivate most prisoners at once, but with priority to motivate lowest motivation first",
                    "fixed bug with animals do not respect reservations (and vice versa)",
                };
                itemsList.Add(itemsArray);
            }
            // 0.8.6
            if (news_0_8_6 || showAll)
            {
                titlesList.Add("Prison Labor Beta v0.8.6");
                string[] itemsArray =
                {
                    "[img]NewsElement_Locks[/img]<b>Locks mod:</b>\nIf you want to allow prisoners to pass by closed doors, please check out my other mod called <b>Locks</b>",
                    "[gap]",
                    "fixed bug that Sowing job do not comply to Labor Area",
                    "fixed bug with JoyGiver debris (sorry about that)",
                    "reduced number of null reference errors with OnGui() method (fixed in v 0.8.5)",
                    "single warden will be able to maintain 7 prisoners, instead of 5 (because of laziness rate reduction) (changed in v 0.8.5)",
                    "decreased laziness rate to 0.002, instead of 0.003 (prisoners will get lazy 1.5x slower) (changed in v 0.8.5)",
                    "decreased manipulation to 70% (instead of 80%) (changed in v 0.8.5)",
                    "fixed null reference exception at loading game (fixed in v 0.8.4)",
                };
                itemsList.Add(itemsArray);
            }
            // 0.8.3
            if (news_0_8_3 || showAll)
            {
                titlesList.Add("Prison Labor Beta v0.8.3");
                string[] itemsArray =
                {
                    "fixed bugs with disabling mod(now you can safely disable mod again)",
                    "fixed bug with prioritizing work",
                    "fixed bug with rendering icons on world map",
                };
                itemsList.Add(itemsArray);
            }
            // 0.8.1
            if (news_0_8_1 || showAll)
            {
                titlesList.Add("Prison Labor Beta v0.8.1");
                string[] itemsArray =
                {
                    "[subtitle] Sorry for any inconvenience caused by 0.8.0 update. Some part of mod are very vulnerable to any mods installed",
                    "[subtitle] If you encouter any bugs <b>please report it on github</b>. I'm fixing most important ones every day. This is (recently) beta version and it has to consist some bugs. Thank you for understaning.",
                    "[subtitle] Also you can always <b>download old version</b> via github, but I think this was last big update",
                    "re-enabled button in Bills detail panel",
                    "added slider to Bills (temporary fix)",
                    "fixed Bill \"Prisoner only\" button (I think, let me know if you still experience errors)",
                    "fixed prisoners aren't working when Motivation is disabled (via Settings)",
                    "fixed null-reference error on some revolts incidents",
                };
                itemsList.Add(itemsArray);
            }
            // 0.8.0
            if (news_0_8_0 || showAll)
            {
                titlesList.Add("Prison Labor Beta v0.8.0");
                string[] itemsArray =
                {
                    "[subtitle]<b>Now in Beta!</b> I would no longer add any more features. Instead I will focus on improving existing ones.",
                    "[gap]",
                    "[subtitle]<b>Main changes:</b>",
                    "[img]NewsElement_Revolt[/img]<b>Revolts:</b>\nPrisoners will now form organized group under self-elected leader if motivation of prisoners is low. They will try to inflict damage to your colony or they will attemp running to elected enemy faction",
                    "[img]NewsElement_InspirationReworked[/img]<b>Insiration reworked:</b>\nQuicker, better and more intuitive.\nYou can now send your prisoners to work outside your walls, but be carefull: they will try to escape if left alone. Prisoners will start thinking about escape after being left for some time.",
                    "[img]LaborAreaExpand[/img]<b>Labor area:</b>\nYou can now select area for labor only. Your colonists will no longer go mine with peasants.\nTo access this tool look into \"Architect->Zones\" panel.",
                    "[img]NewsElement_PrisonersOnly[/img]<b>Prisoners Only button</b>\nGo to Bill details to mark bills for prisoners only!",
                    "[img]NewsElement_WorkAndRecruit[/img]<b>Work and recruit:</b>\nThis feature has been mostly requested by community. I hope it will be well received.",
                    "[gap]",
                    "[subtitle]<b>Other changes:</b>",
                    "added default prisoner interaction mode option to settings menu",
                    "added icons above prisoners indicating whenever he's being motivated/inspired",
                    "reduced manipulation capability of prisoners (now they have 80% of normal manipulation, down from 100%)",
                    "added tutorials triggers (now all tutorials will be shown)",
                    "added watched tutorials to properties (tutorials will no longer be shown after reenabling mod)",
                    "fixed forbidden bug with harvesting plants (again)",
                    "fixed Toil reservation bug (not respecting prisoners' job)",
                    "fixed compatibility with Dubs Hygiene Mod",
                    "fixed SeedsPlease compatibility",
                    "excluded supervising from labor",
                    "rewritten news dialog - now with images and stuff",
                    "perfomance and code improvements",
                    "translation improvements",
                    "[gap]",
                    "[subtitle]Also I want to annouce that I will start new mod called <b>Prison Expansion</b> that would be PrisonExtensions remake.The aim of this mod would be improving Prison Labor experience, especially cell doors and fences.",
                };
                itemsList.Add(itemsArray);
            }
            // 0.7
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
            // 0.6
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
            // 0.5
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
                for (int j = 0; j < items[i].Length; j++)
                {
                    // Draw Image with Text
                    if (items[i][j].StartsWith("[img]"))
                    {
                        int imgLength = items[i][j].IndexOf("[/img]");
                        var imageString = items[i][j].Substring(5, imgLength - 5);
                        var textToDraw = items[i][j].Substring(imgLength + 6);

                        var content = new GUIContent();
                        content.image = ContentFinder<Texture2D>.Get(imageString, false);
                        content.text = textToDraw;
                        Widgets.Label(viewRect, content);

                        viewRect.y += GuiStyle(Text.Font).CalcHeight(content, viewRect.width);
                    }
                    // Draw Gap
                    else if (items[i][j].StartsWith("[gap]"))
                    {
                        color = GUI.color;
                        GUI.color = GUI.color * new Color(1f, 1f, 1f, 0.4f);
                        Widgets.DrawLineHorizontal(viewRect.x, viewRect.y + +GapHeight * 0.5f, viewRect.width);
                        GUI.color = color;
                        viewRect.y += GapHeight;
                    }
                    // Draw Subtitle (without margin)
                    else if (items[i][j].StartsWith("[subtitle]"))
                    {
                        Widgets.Label(viewRect, items[i][j].Substring(10));
                        viewRect.y += Text.CalcHeight(items[i][j], viewRect.width) + Spacing;
                    }
                    // Draw Text
                    else
                    {
                        viewRect.width -= MarginWidth;
                        Widgets.Label(viewRect, MarginText);
                        viewRect.x += MarginWidth;
                        Widgets.Label(viewRect, items[i][j]);
                        viewRect.x -= MarginWidth;
                        viewRect.y += Text.CalcHeight(items[i][j], viewRect.width) + Spacing;
                        viewRect.width += MarginWidth;
                    }
                }

                // Make gap
                viewRect.y += GapHeight;
            }

            Widgets.EndScrollView();
        }

        private float CalculateHeight(float width)
        {
            float height = 0;
            Text.Font = TitleFont;
            foreach (var item in titles)
            {
                height += Text.CalcHeight(item, width) + Spacing + GapHeight;
            }
            Text.Font = ItemFont;
            foreach (var array in items)
                foreach (var item in array)
                {
                    // Image with Text
                    if (item.StartsWith("[img]"))
                    {
                        int imgLength = item.IndexOf("[/img]");
                        var imageString = item.Substring(5, imgLength - 5);
                        var textToDraw = item.Substring(imgLength + 6);

                        var content = new GUIContent();
                        content.image = ContentFinder<Texture2D>.Get(imageString, false);
                        content.text = textToDraw;


                        height += GuiStyle(Text.Font).CalcHeight(content, width);
                    }
                    // Gap
                    else if (item.StartsWith("[gap]"))
                    {
                        height += GapHeight;
                    }
                    else if (item.StartsWith("[subtitle]"))
                    {
                        height += Text.CalcHeight(item, width) + Spacing;
                    }
                    // Only Text
                    else
                    {
                        height += Text.CalcHeight(item, width - MarginWidth) + Spacing;
                    }
                }
            return height;
        }

        private static GUIStyle GuiStyle(GameFont font)
        {
            GUIStyle gUIStyle;
            switch (font)
            {
                case GameFont.Tiny:
                    gUIStyle = Text.fontStyles[0];
                    break;
                case GameFont.Small:
                    gUIStyle = Text.fontStyles[1];
                    break;
                case GameFont.Medium:
                    gUIStyle = Text.fontStyles[2];
                    break;
                default:
                    return null;
            }
            gUIStyle.alignment = Text.Anchor;
            gUIStyle.wordWrap = Text.WordWrap;
            return gUIStyle;
        }

    }
}