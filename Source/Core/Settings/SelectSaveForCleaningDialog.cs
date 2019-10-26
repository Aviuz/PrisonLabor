using PrisonLabor.Tweaks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Settings
{
    public class SelectSaveForCleaningDialog : Window
    {
        private float maxH;
        private Vector2 position;
        private List<string> saves;

        public SelectSaveForCleaningDialog()
        {
            absorbInputAroundWindow = true;
            doCloseX = true;
            doCloseButton = true;

            saves = new List<string>();
            foreach (var file in GenFilePaths.AllSavedGameFiles)
                saves.Add(Path.GetFileNameWithoutExtension(file.Name));
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listRect = new Rect(inRect.x, inRect.y + 10f, inRect.width, inRect.height - 50f);
            var contentRect = new Rect(0f, 0f, inRect.width - 20f, 24f * saves.Count());
            Widgets.BeginScrollView(listRect, ref this.position, contentRect, true);
            var listing_Standard = new Listing_Standard();
            listing_Standard.Begin(contentRect);

            foreach (var save in saves)
            {
                var lineHeight = Text.LineHeight;

                var rect = listing_Standard.GetRect(30f);

                //if (!tooltip.NullOrEmpty())
                //{
                //    if (Mouse.IsOver(rect))
                //        Widgets.DrawHighlight(rect);
                //    TooltipHandler.TipRegion(rect, tooltip);
                //}

                if (Widgets.ButtonText(rect, save))
                {
                    Find.WindowStack.Add(new CleanSaveDialog(save));
                }
                listing_Standard.Gap(listing_Standard.verticalSpacing);
            }

            maxH = listing_Standard.CurHeight;

            listing_Standard.End();
            Widgets.EndScrollView();
        }
    }
}
