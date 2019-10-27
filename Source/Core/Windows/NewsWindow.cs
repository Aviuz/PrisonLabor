using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Other;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Windows
{
    public class NewsWindow : Window
    {
        // Static Properties
        public static bool AutoShow { get; set; }

        public static bool ShowAll { get; set; }

        public static string LastVersionString { get; set; }

        // Fields
        private string[] entries;
        private Vector2 position;

        public NewsWindow()
        {
            doCloseButton = true;
            doCloseX = true;
            Init();
        }

        public void Init()
        {
            var entriesList = new List<string>();

            if (ShowAll)
            {
                foreach (var patch in NewsProvider.allVersionNotes)
                {
                    entriesList.Add(patch.title);
                    entriesList.AddRange(patch.entries);
                }
            }
            else
            {
                foreach (var patch in NewsProvider.NewsAfterVersion(LastVersionString))
                {
                    entriesList.Add(patch.title);
                    entriesList.AddRange(patch.entries);
                }
            }

            entries = entriesList.ToArray();
        }

        public static void TryShow()
        {
            if (AutoShow && PrisonLaborPrefs.ShowNews)
            {
                Find.WindowStack.Add(new NewsWindow());
                PrisonLaborPrefs.LastVersion = PrisonLaborPrefs.Version;
                PrisonLaborPrefs.Save();
                AutoShow = false;
            }
        }

        public static void ForceShow()
        {
            Find.WindowStack.Add(new NewsWindow());
            PrisonLaborPrefs.LastVersion = PrisonLaborPrefs.Version;
            PrisonLaborPrefs.Save();
            AutoShow = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var displayRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 50f);

            var richListing = new GUI_Components.RichListing();
            richListing.PreRender(displayRect, entries);
            richListing.OnGui(ref position);
        }
    }
}