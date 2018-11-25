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
    public abstract class MainTabWindow_Dual : MainTabWindow
    {
        private const int TopMargin = 12;

        private MainTabWindow_PawnTable colonistTab;
        private MainTabWindow_PawnTable prisonerTab;

        protected IEnumerable<Pawn> colonists => Find.CurrentMap.mapPawns.FreeColonists;
        protected IEnumerable<Pawn> prisoners
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

        protected abstract Type InnerTabType { get; }

        public const int ColonistsTabIndex = 0;
        public const int PrisonersTabIndex = 1;
        private int currentTabIndex = 0;

        public MainTabWindow_Dual()
        {
            colonistTab = Activator.CreateInstance(InnerTabType) as MainTabWindow_PawnTable;
            prisonerTab = Activator.CreateInstance(InnerTabType) as MainTabWindow_PawnTable;
            if (colonistTab == null || prisonerTab == null)
                throw new Exception("PrisonLabor exception: wrong MainTabWindow_PawnTable type");
        }

        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);

            string[] tabs;
            if (prisoners.Count() > 0)
                tabs = new string[] { "PrisonLabor_ColonistsOnlyShort".Translate(), "PrisonLabor_PrisonersOnlyShort".Translate() };
            else
                tabs = new string[] { "PrisonLabor_ColonistsOnlyShort".Translate() };

            Text.Font = GameFont.Small;
            PrisonLaborWidgets.BeginTabbedView(rect, tabs, ref currentTabIndex);
            rect.height -= PrisonLaborWidgets.HorizontalSpacing - TopMargin;
            GUI.BeginGroup(new Rect(0, TopMargin, rect.width, rect.height));
            if (currentTabIndex == ColonistsTabIndex)
            {
                colonistTab.DoWindowContents(rect);
            }
            else if (currentTabIndex == PrisonersTabIndex)
            {
                prisonerTab.DoWindowContents(rect);
            }
            GUI.EndGroup();
            PrisonLaborWidgets.EndTabbedView();
        }

        private bool IsTablesNull()
        {
            var tableField = typeof(MainTabWindow_PawnTable).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var colonistTable = tableField.GetValue(colonistTab);
            var prisonerTable = tableField.GetValue(prisonerTab);
            return colonistTable == null || prisonerTable == null;
        }

        private void CreateBothTables()
        {
            var tableField = typeof(MainTabWindow_PawnTable).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            tableField.SetValue(colonistTab, CreateTable(colonistTab, new Func<IEnumerable<Pawn>>(() => colonists)));
            tableField.SetValue(prisonerTab, CreateTable(prisonerTab, new Func<IEnumerable<Pawn>>(() => prisoners)));
        }

        private PawnTable CreateTable(MainTabWindow_PawnTable pawnTable, Func<IEnumerable<Pawn>> pawnsFunc)
        {
            var tableDef = pawnTable.GetType().GetProperty("PawnTableDef", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(pawnTable, null) as PawnTableDef;
            var bottomSpace = (float)pawnTable.GetType().GetProperty("ExtraBottomSpace", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(pawnTable, null);
            var topSpace = (float)pawnTable.GetType().GetProperty("ExtraTopSpace", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(pawnTable, null);

            return new PawnTable(tableDef, pawnsFunc, UI.screenWidth - (int)(this.Margin * 2f), (int)((float)(UI.screenHeight - 35) - bottomSpace - topSpace - this.Margin * 2f));
        }

        public override void Notify_ResolutionChanged()
        {
            CreateBothTables();
            base.Notify_ResolutionChanged();
        }

        public override void PostOpen()
        {
            if (IsTablesNull())
            {
                CreateBothTables();
            }
            var setDirtyMethod = typeof(MainTabWindow_PawnTable).GetMethod("SetDirty", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            setDirtyMethod.Invoke(colonistTab, new object[] { });
            setDirtyMethod.Invoke(prisonerTab, new object[] { });
            Find.World.renderer.wantedMode = WorldRenderMode.None;
        }

        public override Vector2 RequestedTabSize
        {
            get
            {
                var cSize = colonistTab.RequestedTabSize;
                var pSize = prisonerTab.RequestedTabSize;
                return new Vector2(Math.Max(cSize.x, pSize.x), Math.Max(cSize.y, pSize.y) + TopMargin + PrisonLaborWidgets.TabHeight);
            }
        }

        protected override float Margin
        {
            get
            {
                return 6f;
            }
        }
    }
}
