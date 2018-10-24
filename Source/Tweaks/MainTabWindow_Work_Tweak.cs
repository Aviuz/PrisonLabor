using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor.Tweaks
{
    public class MainTabWindow_Work_Tweak : MainTabWindow_PawnTable
    {
        #region Original/Vanilla content of class
        private const int SpaceBetweenPriorityArrowsAndWorkLabels = 40;

        protected override PawnTableDef PawnTableDef
        {
            get
            {
                return PawnTableDefOf.Work;
            }
        }

        protected override float ExtraTopSpace
        {
            get
            {
                return 40f;
            }
        }

        private void DoManualPrioritiesCheckbox()
        {
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect = new Rect(5f, 5f, 140f, 30f);
            bool useWorkPriorities = Current.Game.playSettings.useWorkPriorities;
            Widgets.CheckboxLabeled(rect, "ManualPriorities".Translate(), ref Current.Game.playSettings.useWorkPriorities, false, null, null, false);
            if (useWorkPriorities != Current.Game.playSettings.useWorkPriorities)
            {
                foreach (Pawn current in PawnsFinder.AllMapsWorldAndTemporary_Alive)
                {
                    if (current.Faction == Faction.OfPlayer && current.workSettings != null)
                    {
                        current.workSettings.Notify_UseWorkPrioritiesChanged();
                    }
                }
            }
            if (!Current.Game.playSettings.useWorkPriorities)
            {
                UIHighlighter.HighlightOpportunity(rect, "ManualPriorities-Off");
            }
        }
        #endregion

        private PawnTable prisonersTable;

        public const int ColonistsTabIndex = 0;
        public const int PrisonersTabIndex = 1;
        private int currentTabIndex = 0;

        protected virtual IEnumerable<Pawn> Prisoners
        {
            get
            {
                foreach (var pawn in Find.CurrentMap.mapPawns.PrisonersOfColony)
                {
                    if (PrisonLaborUtility.LaborEnabled(pawn))
                    {
                        WorkSettings.InitWorkSettings(pawn);
                        yield return pawn;
                    }
                }
            }
        }

        public override void DoWindowContents(Rect rect)
        {
            string[] tabs;
            if (Prisoners.Count() > 0)
                tabs = new string[] { "PrisonLabor_ColonistsOnlyShort".Translate(), "PrisonLabor_PrisonersOnlyShort".Translate() };
            else
                tabs = new string[] { "PrisonLabor_ColonistsOnlyShort".Translate() };

            PrisonLaborWidgets.BeginTabbedView(rect, tabs, ref currentTabIndex);
            if (currentTabIndex == ColonistsTabIndex)
            {
                base.DoWindowContents(rect);
                if (Event.current.type == EventType.Layout)
                {
                    return;
                }
                DoManualPrioritiesCheckbox();
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                Text.Anchor = TextAnchor.UpperCenter;
                Text.Font = GameFont.Tiny;
                Rect rect2 = new Rect(370f, rect.y + 5f, 160f, 30f);
                Widgets.Label(rect2, "<= " + "HigherPriority".Translate());
                Rect rect3 = new Rect(630f, rect.y + 5f, 160f, 30f);
                Widgets.Label(rect3, "LowerPriority".Translate() + " =>");
                GUI.color = Color.white;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else if(currentTabIndex == PrisonersTabIndex)
            {
                SetInitialSizeAndPosition();
                prisonersTable.PawnTableOnGUI(new Vector2(rect.x, rect.y + this.ExtraTopSpace));
                if (Event.current.type == EventType.Layout)
                {
                    return;
                }
                DoManualPrioritiesCheckbox();
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                Text.Anchor = TextAnchor.UpperCenter;
                Text.Font = GameFont.Tiny;
                Rect rect2 = new Rect(370f, rect.y + 5f, 160f, 30f);
                Widgets.Label(rect2, "<= " + "HigherPriority".Translate());
                Rect rect3 = new Rect(630f, rect.y + 5f, 160f, 30f);
                Widgets.Label(rect3, "LowerPriority".Translate() + " =>");
                GUI.color = Color.white;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }
            PrisonLaborWidgets.EndTabbedView();
        }

        private PawnTable CreatePrisonerTable()
        {
            return new PawnTable(
                PawnTableDef,
                new Func<IEnumerable<Pawn>>(() => Prisoners),
                UI.screenWidth - (int)(this.Margin * 2f),
                (int)((float)(UI.screenHeight - 35) - this.ExtraBottomSpace - this.ExtraTopSpace - this.Margin * 2f)
                );
        }

        public override void Notify_ResolutionChanged()
        {
            prisonersTable = CreatePrisonerTable();
            base.Notify_ResolutionChanged();
        }

        public override void PostOpen()
        {
            if (this.prisonersTable == null)
            {
                this.prisonersTable = this.CreatePrisonerTable();
            }
            base.PostOpen();
            Find.World.renderer.wantedMode = WorldRenderMode.None;
        }

        public override Vector2 RequestedTabSize
        {
            get
            {
                if (prisonersTable != null)
                {
                    var pTableSize = new Vector2(this.prisonersTable.Size.x + this.Margin * 2f, this.prisonersTable.Size.y + this.ExtraBottomSpace + this.ExtraTopSpace + this.Margin * 2f);
                    var cTableSize = base.RequestedTabSize;
                    var maxTableSize = new Vector2(Math.Max(pTableSize.x, cTableSize.x), Math.Max(pTableSize.y, cTableSize.y) + PrisonLaborWidgets.TabHeight);
                    return maxTableSize;
                }
                else
                    return base.RequestedTabSize;
            }
        }

    }
}
