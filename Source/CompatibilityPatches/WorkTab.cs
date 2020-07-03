using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using PrisonLabor.Tweaks;
using UnityEngine;
using RimWorld.Planet;
using PrisonLabor.Core;
using PrisonLabor.Core.LaborWorkSettings;

namespace PrisonLabor.CompatibilityPatches
{
    static internal class WorkTab
    {
        static internal void Init()
        {
            if (Check())
                Work();
        }

        static internal bool Check()
        {
            if (DefDatabase<PawnColumnDef>.GetNamed("Mood", false) != null && DefDatabase<PawnColumnDef>.GetNamed("Job", false) != null)
                return true;
            else
                return false;
        }

        static internal void Work()
        {
            Log.Message("WorkTab hotfix initialized");
            var workTab = DefDatabase<MainButtonDef>.GetNamed("Work");
            MainTabWindow_WorkTabMod_Tweak.InnerTabType = MainTabWindow_Work_Tweak.MainTabWindowType;
            workTab.tabWindowClass = typeof(MainTabWindow_WorkTabMod_Tweak);
        }
    }

    public class MainTabWindow_WorkTabMod_Tweak : MainTabWindow
    {
        private const int TopMargin = 12;

        private MainTabWindow_PawnTable pawnTab;

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

        public static Type InnerTabType { get; set; }

        public const int ColonistsTabIndex = 0;
        public const int PrisonersTabIndex = 1;
        private int currentTabIndex = 0;
        private int lastTabIndex = 0;

        public MainTabWindow_WorkTabMod_Tweak()
        {
            pawnTab = Activator.CreateInstance(InnerTabType) as MainTabWindow_PawnTable;
            if (pawnTab == null)
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
            if (currentTabIndex != lastTabIndex)
            {
                CreatePawnTable();
                lastTabIndex = currentTabIndex;
            }
            pawnTab.DoWindowContents(rect);

            GUI.EndGroup();
            PrisonLaborWidgets.EndTabbedView();
        }

        private bool IsTableNull()
        {
            var tableField = typeof(MainTabWindow_PawnTable).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var colonistTable = tableField.GetValue(pawnTab);
            return colonistTable == null;
        }

        private void CreatePawnTable()
        {
            var tableField = typeof(MainTabWindow_PawnTable).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (currentTabIndex == ColonistsTabIndex)
                tableField.SetValue(pawnTab, CreateTable(pawnTab, new Func<IEnumerable<Pawn>>(() => colonists)));
            else if (currentTabIndex == PrisonersTabIndex)
                tableField.SetValue(pawnTab, CreateTable(pawnTab, new Func<IEnumerable<Pawn>>(() => prisoners)));
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
            CreatePawnTable();
            base.Notify_ResolutionChanged();
        }

        public override void PostOpen()
        {
            if (IsTableNull())
            {
                CreatePawnTable();
            }
            var setDirtyMethod = typeof(MainTabWindow_PawnTable).GetMethod("SetDirty", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            setDirtyMethod.Invoke(pawnTab, new object[] { });
            Find.World.renderer.wantedMode = WorldRenderMode.None;
        }

        public override Vector2 RequestedTabSize
        {
            get
            {
                var cSize = pawnTab.RequestedTabSize;
                return new Vector2(Math.Max(cSize.x, 0), Math.Max(cSize.y, 0) + TopMargin + PrisonLaborWidgets.TabHeight);
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
