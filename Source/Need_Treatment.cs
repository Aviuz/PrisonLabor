using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    /// <summary>
    /// VeryGood:   4
    /// Good:       3
    /// Normal:     2
    /// Bad:        1
    /// VeryBad:    0
    /// </summary>
    public enum TreatmentCategory : byte
    {
        VeryGood = 4,
        Good = 3,
        Normal = 2,
        Bad = 1,
        VeryBad = 0,
    }

    public class Need_Treatment : Need
    {
        #region Constants
        public const float ResocializationLevel = 0.1f;

        // 10% every 12 days
        private const float LaborRate = 1f / (120f * GenDate.TicksPerDay / 150f);
        // 1% every 12 days for every point of status
        private const float StatusMultiplier = 1f / (1200f * GenDate.TicksPerDay / 150f);
        // 10% every 6 days
        private const float JoyRate = 1f / (60f * GenDate.TicksPerDay / 150f);

        private const float BeatenHit = -0.1f;
        #endregion

        #region TreshPercentages
        public float PercentageThreshVeryGood => 0.75f;
        public float PercentageThreshGood => 0.5f;
        public float PercentageThreshNormal => 0.25f;
        public float PercentageThreshBad => 0.10f;
        #endregion

        private bool _resocializationReady = false;

        public bool ResocializationReady
        {
            get => _resocializationReady;
            set { _resocializationReady = value; }
        }

        public TreatmentCategory CurCategory
        {
            get
            {
                if (CurLevelPercentage < PercentageThreshBad)
                    return TreatmentCategory.VeryBad;
                else if (CurLevelPercentage < PercentageThreshNormal)
                    return TreatmentCategory.Bad;
                else if (CurLevelPercentage < PercentageThreshGood)
                    return TreatmentCategory.Normal;
                else if (CurLevelPercentage < PercentageThreshVeryGood)
                    return TreatmentCategory.Good;
                else
                    return TreatmentCategory.VeryGood;
            }
        }

        public Need_Treatment(Pawn pawn) : base(pawn) { }

        public override int GUIChangeArrow => 0;

        public static NeedDef Def => PrisonLaborDefOf.PrisonLabor_Treatment;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref _resocializationReady, "PrisonLabor_resocialization_ready", false, false);
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
            statusScore -= (int)pawn.health.hediffSet.PainTotal / 10;
            if (pawn.health.HasHediffsNeedingTend())
                statusScore -= 7;

            CurLevel += statusScore * StatusMultiplier;


            // Labor
            var motivation = pawn.needs.TryGetNeed<Need_Motivation>();
            if (motivation != null && motivation.IsPrisonerWorking)
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
            {
                threshPercents = new List<float>();
                threshPercents.Add(PercentageThreshBad);
                threshPercents.Add(PercentageThreshNormal);
                threshPercents.Add(PercentageThreshGood);
                threshPercents.Add(PercentageThreshVeryGood);
            }
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

        public void NotifyPrisonerBeaten(DamageInfo dinfo, bool absorbed)
        {
            CurLevel += BeatenHit;
        }
    }
}