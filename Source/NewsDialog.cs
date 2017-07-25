using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    class NewsDialog : Window
    {
        private static bool autoShow = true;

        public static bool showAll = false;

        public static bool news_0_5 = false;
        public static bool news_0_6 = false;
        public static bool news_0_7 = false;

        private Vector2 position;
        private Rect cRect;

        public NewsDialog()
        {
            this.doCloseButton = true;
            this.doCloseX = true;
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
            if(cRect == null)
                cRect = inRect;

            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 50);

            Widgets.BeginScrollView(viewRect, ref this.position, cRect, true);

            float CurHeight = 0;

            Listing_Standard ls_title = new Listing_Standard(GameFont.Medium);
            Listing_Standard ls_desc = new Listing_Standard(GameFont.Small);
            if(news_0_7 || showAll)
            {
                ls_title.Begin(new Rect(cRect.x, cRect.y + CurHeight, cRect.width, cRect.height - CurHeight));
                ls_title.Label("Prison Labor Alpha v0.7");
                ls_title.GapLine();
                ls_title.End();
                CurHeight += ls_title.CurHeight;
                ls_desc.Begin(new Rect(cRect.x, cRect.y + CurHeight, cRect.width, cRect.height - CurHeight));
                ls_desc.Label(" - Added settings! You can now change almost any aspect of this mod, including:\n   * work types\n   * motivation mechanics\n   * prevention of planting advanced plants.");
                ls_desc.Label(" - Added \"uninstaller\" (\"disable\" option in settings), which will allow to disable this mod from existing saves.");
                ls_desc.Label(" - \"No more beeping!\". Changed way of informing player what's going on with prisoners. It should be less annoying and more insightful.");
                ls_desc.Label(" - Fixed bugs, including bug that prevents prisoners from cleaning and bug that causes warden to stuck in loop of delivering food to prisoner.");
                ls_desc.Label(" - \"No more watching while prisoner is sleeping.\"Wardens will no longer watch over not working prisoners.");
                ls_desc.Label(" - Prisoners will now stay in bed while waiting for operation");
                ls_desc.Label(" - Prisoners will now stop work when starving for default (\"Anything\" time), instead of hungry. They will still get minor debuff.");
                ls_desc.Gap();
                ls_desc.End();
                CurHeight += ls_desc.CurHeight;
            }
            if(news_0_6 || showAll)
            {
                ls_title.Begin(new Rect(cRect.x, cRect.y + CurHeight, cRect.width, cRect.height - CurHeight));
                ls_title.Label("Prison Labor Alpha v0.6");
                ls_title.GapLine();
                ls_title.End();
                CurHeight += ls_title.CurHeight;
                ls_desc.Begin(new Rect(cRect.x, cRect.y + CurHeight, cRect.width, cRect.height - CurHeight));
                ls_desc.Label("Changes in PrisonLabor v0.6:\n\n   1. Time restrictions - now you can manage your prisoners time for sleep, work and joy. You can now even force them to work when they're hungry!\n   2. Getting food by prisoners - Now prisoners will look for food in much better way, and now (when they desperate enough) they will eat corpses!\n   3. \"Laziness\" changed to \"Motivation\" and inverted.\n\n   ATTENTION: After PrisonLabor reaches beta all saves with PrisonLabor v0.5a or lower will be corrupted and unplayable. This version (0.6) is safe and converts all older saves.");
                ls_desc.Gap();
                ls_desc.End();
                CurHeight += ls_desc.CurHeight;
            }
            if(news_0_5 || showAll)
            {
                ls_title.Begin(new Rect(cRect.x, cRect.y + CurHeight, cRect.width, cRect.height - CurHeight));
                ls_title.Label("Prison Labor Alpha v0.5");
                ls_title.GapLine();
                ls_title.End();
                CurHeight += ls_title.CurHeight;
                ls_desc.Begin(new Rect(cRect.x, cRect.y + CurHeight, cRect.width, cRect.height - CurHeight));
                ls_desc.Label("Major changes to PrisonLabor:\n\n   1. Prisoners can now grow, but only plants that not require any skills.\n   2. You can now manage prisoners work types. Just check \"Work\" tab!\n   3. Laziness now appear on \"Needs\" tab. Above 50% wardens will watch prisoners. Above 80% prisoners won't work unless supervised.\n   4. Wardens will now bring food to prisoners that went too far from his bed.\n   5. Prisoners won't gain laziness when not working anymore.\n   6. Fixed many bugs");
                ls_desc.Gap();
                ls_desc.End();
                CurHeight += ls_desc.CurHeight;
            }

            Widgets.EndScrollView();

            cRect = new Rect(inRect.x, inRect.y, inRect.width - 50f, CurHeight + 50f);
        }
    }
}
