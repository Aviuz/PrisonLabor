using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PrisonLabor
{
    class SelectWorkTypesDialog : Window
    {
        Dictionary<WorkTypeDef, bool> workTypes;

        float maxH;
        Vector2 position;
        bool temp;

        public SelectWorkTypesDialog()
        {
            this.absorbInputAroundWindow = true;
            this.closeOnEscapeKey = true;
            this.doCloseX = true;
            this.doCloseButton = true;

            workTypes = new Dictionary<WorkTypeDef, bool>();

            foreach (WorkTypeDef workType in DefDatabase<WorkTypeDef>.AllDefs)
                if (!PrisonLaborUtility.WorkDisabled(workType))
                    workTypes.Add(workType, true);
                else
                    workTypes.Add(workType, false);
        }

        public override void DoWindowContents(Rect inRect)
        {

            Rect listRect = new Rect(inRect.x, inRect.y + 10f, inRect.width, inRect.height - 50f);
            Rect contentRect = new Rect(0f, 0f, inRect.width - 20f, 24f * workTypes.Count());
            Widgets.BeginScrollView(listRect, ref this.position, contentRect, true);
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(contentRect);

            WorkTypeDef workTypeClicked = null;
            foreach (WorkTypeDef workDef in workTypes.Keys)
            {
                String label = workDef.labelShort, tooltip = workDef.description;
                float lineHeight = Text.LineHeight;
                bool checkOn = workTypes[workDef];
                //workTypes.TryGetValue(workDef, out checkOn);
                Rect rect = listing_Standard.GetRect(lineHeight);
                if (!tooltip.NullOrEmpty())
                {
                    if (Mouse.IsOver(rect))
                    {
                        Widgets.DrawHighlight(rect);
                    }
                    TooltipHandler.TipRegion(rect, tooltip);
                }
                TextAnchor anchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rect, label);
                if (Widgets.ButtonInvisible(rect, false))
                {
                    workTypeClicked = workDef;
                    if (checkOn)
                    {
                        SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera(null);
                    }
                    else
                    {
                        SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera(null);
                    }
                }
                Color color = GUI.color;
                Texture2D image;
                if (checkOn)
                {
                    image = Widgets.CheckboxOnTex;
                }
                else
                {
                    image = Widgets.CheckboxOffTex;
                }
                Rect position = new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f);
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
            List<WorkTypeDef> list = new List<WorkTypeDef>();
            foreach (WorkTypeDef workDef in workTypes.Keys)
            {
                if(workTypes[workDef] == true)
                {
                    list.Add(workDef);
                }
            }
            PrisonLaborUtility.SetAllowedWorkTypes(list);
            PrisonLaborPrefs.AllowedWorkTypes = PrisonLaborUtility.AllowedWorkTypesData;
        }
    }
}
