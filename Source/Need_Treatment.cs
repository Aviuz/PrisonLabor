using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    public class Need_Treatment : Need
    {
        private const float LaborRate = -0.01f;
        private const float StatusMultiplier = 0.01f;
        private const float JoyRate = 0.03f;

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
            // Joy
            if (pawn.timetable != null && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                CurLevel += JoyRate;

            // Status
            var hunger = pawn.needs.TryGetNeed<Need_Food>();
            
            int statusScore = 0;
            if (hunger.CurCategory < HungerCategory.UrgentlyHungry)
                statusScore += 5;
            if (hunger.CurCategory < HungerCategory.Hungry)
                statusScore += 5;
            statusScore -= (int)pawn.health.hediffSet.PainTotal/10;
            if (pawn.health.HasHediffsNeedingTend())
                statusScore -= 7;

            CurLevel += statusScore * StatusMultiplier;


            // Labor
            var motivation = pawn.needs.TryGetNeed<Need_Motivation>();
            if (motivation != null && motivation.PrisonerWorking)
                CurLevel += LaborRate;
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