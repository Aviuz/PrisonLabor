using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Windows
{
    public class ReplayTutorialsWindow : Window
    {
        public ReplayTutorialsWindow()
        {
            doCloseButton = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing_panel = new Listing_Standard();

            listing_panel.Begin(inRect);

            if (listing_panel.ButtonTextLabeled("test", "PrisonLabor_ShowTutorialButton".Translate()))
            {
                TutorialWindow.Show("test");
            }

            if (listing_panel.ButtonTextLabeled("test2", "PrisonLabor_ShowTutorialButton".Translate()))
            {
                TutorialWindow.Show("test2");
            }

            listing_panel.End();
        }

        public static void Show()
        {
            Find.WindowStack.Add(new ReplayTutorialsWindow());
        }
    }
}
