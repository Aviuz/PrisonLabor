using System.Collections.Generic;
using System.Text;
using PrisonLabor.Constants;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Needs
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
        private bool _resocializationReady = false;

        public float PercentageThreshVeryGood => 0.85f;
        public float PercentageThreshGood => 0.65f;
        public float PercentageThreshNormal => 0.35f;
        public float PercentageThreshBad => 0.15f;

        public override int GUIChangeArrow => 0;

        public static NeedDef Def => PL_DefOf.PrisonLabor_Treatment;

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

        public static bool ShowOnList
        {
            get
            {
                return PL_DefOf.PrisonLabor_Treatment.showOnNeedList;
            }
            set
            {
                PL_DefOf.PrisonLabor_Treatment.showOnNeedList = value;
            }
        }

        public Need_Treatment(Pawn pawn) : base(pawn) { }

        public override void SetInitialLevel()
        {
            CurLevelPercentage = 0.5f;
        }

        public override void NeedInterval()
        {
            // Joy
            if (pawn.timetable != null && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                CurLevel += BGP.JoyRate;

            // Status
            if (pawn.needs.food != null)
            {
                var hunger = pawn.needs.TryGetNeed<Need_Food>();

                int statusScore = 0;
                if (hunger.CurCategory < HungerCategory.UrgentlyHungry)
                    statusScore += 5;
                if (hunger.CurCategory < HungerCategory.Hungry)
                    statusScore += 5;
                statusScore -= (int)pawn.health.hediffSet.PainTotal / 10;
                if (pawn.health.HasHediffsNeedingTend())
                    statusScore -= 7;

                CurLevel += statusScore * BGP.StatusMultiplier;
            }


            // Labor
            var motivation = pawn.needs.TryGetNeed<Need_Motivation>();
            if (motivation != null && motivation.IsPrisonerWorking)
                CurLevel += BGP.LaborRate;

        }

        public override string GetTipString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetTipString());
            return stringBuilder.ToString();
        }

        public void NotifyPrisonerBeaten(DamageInfo dinfo, bool absorbed)
        {
            CurLevel += BGP.BeatenHit;
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true, Rect? rectForTooltip = null)
        {
            if (threshPercents == null)
            {
                threshPercents = new List<float>();
                threshPercents.Add(PercentageThreshBad);
                threshPercents.Add(PercentageThreshNormal);
                threshPercents.Add(PercentageThreshGood);
                threshPercents.Add(PercentageThreshVeryGood);
            }
            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip, rectForTooltip);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref _resocializationReady, "PrisonLabor_resocialization_ready", false, false);
        }
    }
}