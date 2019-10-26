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

            if (listing_panel.ButtonTextLabeled("Indroduction", "PrisonLabor_ShowTutorialButton".Translate()))
            {
                TutorialWindow.Show("Indroduction");
            }

            if (listing_panel.ButtonTextLabeled("Labor Area", "PrisonLabor_ShowTutorialButton".Translate()))
            {
                TutorialWindow.Show("LaborArea");
            }

            if (listing_panel.ButtonTextLabeled("Treatment system", "PrisonLabor_ShowTutorialButton".Translate()))
            {
                TutorialWindow.Show("Treatment");
            }

            listing_panel.End();
        }

        public static void Show()
        {
            Find.WindowStack.Add(new ReplayTutorialsWindow());
        }
    }
}
