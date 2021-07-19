using PrisonLabor.Core.LaborWorkSettings;
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
                Vector2 size = new Vector2(0, 0);
                foreach(var tab in tabsView.Values)
                {                    
                    size.x = Math.Max(size.x, tab.RequestedTabSize.x);
                    size.y = Math.Max(size.y, tab.RequestedTabSize.y);
                }
                return new Vector2(size.x + Margin * 2f, size.y + ExtraBottomSpace + ExtraTopSpace + Margin * 2f);
            }
        }


        protected IEnumerable<Pawn> AllColonyPrisoners => Find.CurrentMap.mapPawns.PrisonersOfColony;
 

        protected IEnumerable<Pawn> WorkingPrisoners
        {
            get
            {
                foreach (var pawn in AllColonyPrisoners)
                {
                    if (PrisonLaborUtility.LaborEnabled(pawn))
                    {
                        WorkSettings.InitWorkSettings(pawn);
                        yield return pawn;
                    }
                }
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
           foreach(var def in DefDatabase<PrisonersTabDef>.AllDefs)
           {
                if(def.dev == false)
                {
                    tabsView.Add(def, CreateWindow(def));
                }
                if (def.dev == true && Prefs.DevMode)
                {
                    tabsView.Add(def, CreateWindow(def));
                }

            }
           DebugLogger.debug("Prisoners main windows constructed");
            
        }
        public override void DoWindowContents(Rect inRect)
        {           
            
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;   
            
            inRect.yMin += 32f;
            Widgets.DrawMenuSection(inRect);
            TabDrawer.DrawTabs(inRect, tabs);
            inRect.yMin += TabDrawer.TabHeight;           
            GetTable(CurTab).DoWindowContents(inRect);
        }
        public override void PostOpen()
        {
            base.PostOpen();
            tabs.Clear();
            foreach (PrisonersTabDef tabDef in DefDatabase<PrisonersTabDef>.AllDefs.OrderBy(def => def.order))
            {                
                tabs.Add(new PrisonerWindowTab(tabDef, tabDef.LabelCap, delegate
                {
                    CurTab = tabDef;
                }, () => CurTab == tabDef));
                GetTable(tabDef);
            }
            CurTab = tabs[0].def;
            foreach(var tab in tabsView.Values)
            {
                tab.PostOpen();
            }

        }

        private CustomTabWindow GetTable(PrisonersTabDef def)
        {

            if (!tabsView.TryGetValue(def, out CustomTabWindow table))
            {
                table = CreateWindow(def);
                tabsView.Add(def, table);
            }            
            return table;
        }

        private CustomTabWindow CreateWindow(PrisonersTabDef def)
        {
            CustomTabWindow window = (CustomTabWindow)Activator.CreateInstance(def.workerClass);
            window.PostOpen();
            return window;
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
