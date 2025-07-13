using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class PrisonerButtonWindow : MainTabWindow
    {
        private List<PrisonerWindowTab> tabs = new List<PrisonerWindowTab>();
        private class PrisonerWindowTab : TabRecord
        {
            public readonly PrisonersTabDef def;
            public PrisonerWindowTab(PrisonersTabDef def, string label, Action clickedAction, Func<bool> selected)
                : base(label, clickedAction, selected)
            {
                this.def = def;
            }

        }


        public override Vector2 RequestedTabSize
        {
            get
            {
                var size = new Vector2(1230f, 155f);
                foreach(var tab in tabsView.Where(kv => ShouldShowTab(kv.Key)).Select(kv => kv.Value))
                {                    
                    size.x = Math.Max(size.x, tab.RequestedTabSize.x);
                    size.y = Math.Max(size.y, tab.RequestedTabSize.y);
                }
                return new Vector2(size.x + Margin * 2f, size.y + ExtraBottomSpace + ExtraTopSpace + Margin * 2f);
            }
        }

        private PrisonersTabDef curTabInt;

        private PrisonersTabDef CurTab
        {
            get
            {
                return curTabInt;
            }
            set
            {
                if (value != curTabInt)
                {
                    curTabInt = value;
                }
            }
        }
        private Dictionary<PrisonersTabDef, CustomTabWindow> tabsView = new Dictionary<PrisonersTabDef, CustomTabWindow>();
        protected virtual float ExtraBottomSpace => 53f;
        protected override float Margin => 6f;
        protected virtual float ExtraTopSpace => 0f;

        public PrisonerButtonWindow() : base()
        {
           foreach(var tabDef in DefDatabase<PrisonersTabDef>.AllDefs)
           {
                DebugLogger.debug($"Def: {tabDef.defName}, def.dev: {tabDef.dev}, dev: {Prefs.DevMode}");
                tabsView[tabDef] = CreateWindow(tabDef);
           }
           DebugLogger.debug("Prisoners main windows constructed");            
        }

        public override void DoWindowContents(Rect inRect)
        {           
            inRect.yMin += TabDrawer.TabHeight;
            Widgets.DrawMenuSection(inRect);
            TabDrawer.DrawTabs(inRect, tabs);
            inRect.yMin += Margin;           
            if (Event.current.type != EventType.Layout)
            {
                GetTable(CurTab).DoWindowContents(inRect);
            }
        }
        public override void PostOpen()
        {
            base.PostOpen();
            tabs.Clear();
            foreach (var tabDef in tabsView.Keys.Where(ShouldShowTab).OrderBy(d => d.order))
            {                
                tabs.Add(new PrisonerWindowTab(tabDef, tabDef.LabelCap, delegate
                {
                    GetTable(tabDef).PostOpen();
                    CurTab = tabDef;
                }, () => CurTab == tabDef));
            }

            if (CurTab == null)
            {
                CurTab = tabs[1].def;
            }
            GetTable(CurTab).PostOpen();
        }

        private static bool ShouldShowTab(PrisonersTabDef tabDef)
        {
            return !tabDef.dev || tabDef.dev == Prefs.DevMode;
        }

        private CustomTabWindow GetTable(PrisonersTabDef tabDef)
        {
            if (tabsView.TryGetValue(tabDef, out var table))
            {
                return table;
            }
            table = CreateWindow(tabDef);
            tabsView.Add(tabDef, table);
            return table;
        }

        private static CustomTabWindow CreateWindow(PrisonersTabDef tabDef)
        {
            return (CustomTabWindow)Activator.CreateInstance(tabDef.workerClass);
        }
        public void Notify_PawnsChanged()
        {
            foreach (var tab in tabsView.Values)
            {
                tab.Notify_PawnsChanged();
            }
            base.SetInitialSizeAndPosition();
        }

    }
}
