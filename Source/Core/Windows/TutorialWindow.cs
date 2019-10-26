using PrisonLabor.Core.Other;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Windows
{
    public class TutorialWindow : Window
    {
        // Fields
        private string[] entries;
        private Vector2 position;

        public override Vector2 InitialSize => new Vector2(800, 700);

        public TutorialWindow(string tutorialKey)
        {
            entries = TutorialProvider.TutorialNodes[tutorialKey].entries;

            doCloseButton = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var displayRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 50f);

            var richListing = new GUI_Components.RichListing();
            richListing.PreRender(displayRect, entries);
            richListing.OnGui(ref position);
        }

        public static void Show(string key)
        {
            Find.WindowStack.Add(new TutorialWindow(key));
        }
    }
}
