using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    public class Need_Treatment : Need
    {
        private static NeedDef def;

        public Need_Treatment(Pawn pawn) : base(pawn)
        {
        }

        public override int GUIChangeArrow => 0;

        public static NeedDef Def
        {
            get
            {
                if (def == null)
                    def = DefDatabase<NeedDef>.GetNamed("PrisonLabor_Treatment");
                return def;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override void NeedInterval()
        {

        }

        public override void SetInitialLevel()
        {
            CurLevelPercentage = 0.5f;
        }

        public override string GetTipString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetTipString());
            return stringBuilder.ToString();
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f,
            bool drawArrows = true, bool doTooltip = true)
        {
            if (threshPercents == null)
                threshPercents = new List<float>();
            threshPercents.Clear();
            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
        }


        public static bool ShowOnList
        {
            get
            {
                return PrisonLaborDefOf.PrisonLabor_Treatment.showOnNeedList;
            }
            set
            {
                PrisonLaborDefOf.PrisonLabor_Treatment.showOnNeedList = value;
            }
        }
    }
}