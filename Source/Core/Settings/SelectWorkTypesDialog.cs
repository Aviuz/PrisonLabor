using System.Collections.Generic;
using System.Linq;
using PrisonLabor.Core.LaborWorkSettings;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PrisonLabor.Core.Settings
{
    public class SelectWorkTypesDialog : Window
    {
        private float maxH;
        private Vector2 position;
        private readonly Dictionary<WorkTypeDef, bool> workTypes;

        public SelectWorkTypesDialog()
        {
            absorbInputAroundWindow = true;
            doCloseX = true;
            doCloseButton = true;

            workTypes = new Dictionary<WorkTypeDef, bool>();

            foreach (var workType in WorkSettings.AvailableWorkTypes)
                if (!WorkSettings.WorkDisabled(workType))
                    workTypes.Add(workType, true);
                else
                    workTypes.Add(workType, false);
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listRect = new Rect(inRect.x, inRect.y + 10f, inRect.width, inRect.height - 50f);
            var contentRect = new Rect(0f, 0f, inRect.width - 20f, 24f * workTypes.Count());
            Widgets.BeginScrollView(listRect, ref this.position, contentRect, true);
            var listing_Standard = new Listing_Standard();
            listing_Standard.Begin(contentRect);

            WorkTypeDef workTypeClicked = null;
            foreach (var workDef in workTypes.Keys)
            {
                string label = workDef.labelShort, tooltip = workDef.description;
                var lineHeight = Text.LineHeight;
                var checkOn = workTypes[workDef];
                //workTypes.TryGetValue(workDef, out checkOn);
                var rect = listing_Standard.GetRect(lineHeight);
                if (!tooltip.NullOrEmpty())
                {
                    if (Mouse.IsOver(rect))
                        Widgets.DrawHighlight(rect);
                    TooltipHandler.TipRegion(rect, tooltip);
                }
                var anchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rect, label);
                if (Widgets.ButtonInvisible(rect, false))
                {
                    workTypeClicked = workDef;
                    if (checkOn)
                        SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                    else
                        SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
                var color = GUI.color;
                Texture2D image;
                if (checkOn)
                    image = Widgets.CheckboxOnTex;
                else
                    image = Widgets.CheckboxOffTex;
                var position = new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f);
                GUI.DrawTexture(position, image);
                Text.Anchor = anchor;
                listing_Standard.Gap(listing_Standard.verticalSpacing);
            }

            if (workTypeClicked != null)
            {
                workTypes[workTypeClicked] = !workTypes[workTypeClicked];
                Apply(workTypes);
            }

            maxH = listing_Standard.CurHeight;

            listing_Standard.End();
            Widgets.EndScrollView();
        }

        private static void Apply(Dictionary<WorkTypeDef, bool> workTypes)
        {
            var list = new List<WorkTypeDef>();
            foreach (var workDef in workTypes.Keys)
                if (workTypes[workDef])
                    list.Add(workDef);
            WorkSettings.AllowedWorkTypes = list;
        }
    }
}