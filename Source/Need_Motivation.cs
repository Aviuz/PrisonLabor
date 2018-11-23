using System.Collections.Generic;
using System.Text;
using PrisonLabor.HarmonyPatches;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    public class Need_Motivation : Need
    {
        public const float InspireRate = 0.015f;
        public const int WardenCapacity = (int)(InspireRate / LazyRate);
        public const float InpirationRange = 10.0f;

        private const float LazyLevel = 0.2f;
        private const float NeedInspirationLevel = 0.5f;
        private const int ReadyToRunLevel = 100;

        private const float LazyRate = 0.002f;
        private const float HungryRate = 0.006f;
        private const float TiredRate = 0.006f;
        private const float HealthRate = 0.006f;
        private const float JoyRate = 0.001f;

        private int escapeTimerTicks;

        public int ReadyToRunPercentage => escapeTimerTicks * 100 / ReadyToRunLevel;

        public float PercentageThreshNeedInsipration => NeedInspirationLevel;

        public float PercentageThreshLazy => LazyLevel;
        //TODO change to lazy category?
        public HungerCategory CurCategory => 0;

        public static NeedDef Def => PrisonLaborDefOf.PrisonLabor_Motivation;

        public bool Motivated => _GUIChangeArrow > 0;

        private int _GUIChangeArrow;
        public override int GUIChangeArrow => _GUIChangeArrow;

        /// <summary>
        /// Indicates whenever pawn need motivation source to work
        /// </summary>
        public bool NeedToBeMotivated { get; private set; }

        /// <summary>
        /// Indicates whenever pawn is currently working, and his motivation is decreasing by laziness rate.
        /// </summary>
        public bool IsPrisonerWorking { get; set; }

        /// <summary>
        /// Indicates whenever pawn is lazy and stopped working by lack of motivation
        /// </summary>
        public bool IsLazy { get; private set; }

        /// <summary>
        /// Indicates that pawn is getting motivation from working jailor
        /// </summary>
        public bool Watched { get; private set; }

        private bool _ReadyToEscape;
        public bool ReadyToEscape
        {
            get => _ReadyToEscape;

            private set
            {
                if (DevTools.LogEscapeUtilityEnabled && _ReadyToEscape != value)
                    Log.Message($"{pawn.Name.ToStringShort}.ReadyToRun = {value}");
                _ReadyToEscape = value;
            }
        }

        bool _CanEscape;
        public bool CanEscape
        {
            get => _CanEscape;

            set
            {
                if (DevTools.LogEscapeUtilityEnabled && _CanEscape != value)
                    Log.Message($"{pawn.Name.ToStringShort}.CanEscape = {value}");
                _CanEscape = value;
            }
        }

        public Need_Motivation(Pawn pawn) : base(pawn) { }

        public override void SetInitialLevel()
        {
            CurLevelPercentage = 1.0f;
            IsPrisonerWorking = false;
        }

        public override void NeedInterval()
        {
            // Update value of need
            CurLevel += GetChangePoints();

            // Set NeedToBeMotivated
            if (CurLevel == MaxLevel)
                NeedToBeMotivated = false;
            else if (CurLevel <= NeedInspirationLevel)
                NeedToBeMotivated = true;

            // Set IsLazy
            if (CurLevel <= LazyLevel && _GUIChangeArrow <= 0)
                IsLazy = true;
            else
                IsLazy = false;

            // Set ReadyToEscape
            ImpatientTick();
        }

        private void ImpatientTick()
        {
            if (Watched || !CanEscape)
            {
                if (escapeTimerTicks != 0)
                {
                    escapeTimerTicks = 0;
                    ReadyToEscape = false;
                }
            }
            else if (!ReadyToEscape)
            {
                escapeTimerTicks++;
                if (escapeTimerTicks >= ReadyToRunLevel)
                {
                    ReadyToEscape = true;
                }
            }
        }

        private float GetChangePoints()
        {
            if (pawn.IsPrisoner && pawn.IsPrisonerOfColony)
            {
                if (pawn.GetRoomGroup() != null)
                {
                    var value = InspirationUtility.GetInsiprationValue(pawn);

                    if (value != 0)
                        Watched = true;
                    else
                        Watched = false;

                    if (PrisonLaborUtility.LaborEnabled(pawn))
                    {
                        if (IsPrisonerWorking)
                        {
                            value -= LazyRate;
                            if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
                                value -= HealthRate;
                            value -= (int)pawn.needs.food.CurCategory * HungryRate;
                            value -= (int)pawn.needs.rest.CurCategory * TiredRate;
                        }
                        else if (pawn.timetable != null && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                        {
                            value += JoyRate;
                        }
                    }

                    _GUIChangeArrow = value.CompareTo(0.0f);
                    return value;
                }
                else
                {
                    _GUIChangeArrow = 0;
                    return 0.0f;
                }
            }
            else
            {
                _GUIChangeArrow = 1;
                return +0.01f;
            }
        }

        public override string GetTipString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetTipString());
            stringBuilder.AppendLine();
            stringBuilder.Append("PrisonLabor_WardenResponseThreshold".Translate());
            stringBuilder.AppendLine($": {PercentageThreshNeedInsipration.ToStringPercent()}");
            stringBuilder.Append("PrisonLabor_StoppingWorkThreshold".Translate());
            stringBuilder.AppendLine($": {PercentageThreshLazy.ToStringPercent()}");
            return stringBuilder.ToString();
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true)
        {
            if (threshPercents == null)
                threshPercents = new List<float>();
            threshPercents.Clear();
            threshPercents.Add(PercentageThreshLazy);
            threshPercents.Add(PercentageThreshNeedInsipration);
            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref escapeTimerTicks, "EscapeTimer", 0, false);
        }
    }
}