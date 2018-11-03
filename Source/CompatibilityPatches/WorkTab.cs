using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using PrisonLabor.Tweaks;
using UnityEngine;
using RimWorld.Planet;

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
        private MainTabWindow_PawnTable singleTab;

        protected IEnumerable<Pawn> colonistsAndPrisoners
        {
            get
            {
                foreach (var pawn in Find.CurrentMap.mapPawns.FreeColonists)
                    yield return pawn;
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

        public MainTabWindow_WorkTabMod_Tweak()
        {
            singleTab = Activator.CreateInstance(InnerTabType) as MainTabWindow_PawnTable;
            if (singleTab == null)
                throw new Exception("PrisonLabor exception: wrong MainTabWindow_PawnTable type");
        }

        public override void DoWindowContents(Rect rect)
        {
            singleTab.DoWindowContents(rect);
        }

        private bool IsTablesNull()
        {
            var tableField = typeof(MainTabWindow_PawnTable).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var singleTable = tableField.GetValue(singleTab);
            return singleTable == null;
        }

        private void CreateBothTables()
        {
            var tableField = typeof(MainTabWindow_PawnTable).GetField("table", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            tableField.SetValue(singleTab, CreateTable(singleTab, new Func<IEnumerable<Pawn>>(() => colonistsAndPrisoners)));
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
            setDirtyMethod.Invoke(singleTab, new object[] { });
            Find.World.renderer.wantedMode = WorldRenderMode.None;
            SetInitialSizeAndPosition();
        }

        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(singleTab.RequestedTabSize.x+1, singleTab.RequestedTabSize.y);
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
