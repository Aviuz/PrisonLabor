using System.Collections.Generic;
using System.Text;
using PrisonLabor.Constants;
using PrisonLabor.Core.Trackers;
using PrisonLabor.HarmonyPatches;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Needs
{
    public class Need_Motivation : Need
    {
        private const float LazyLevel = 0.2f;
        private const float NeedInspirationLevel = 0.5f;

        public float PercentageThreshNeedInsipration => NeedInspirationLevel;

        public float PercentageThreshLazy => LazyLevel;
        //TODO change to lazy category?
        public HungerCategory CurCategory => 0;

        public static NeedDef Def => PL_DefOf.PrisonLabor_Motivation;

        public bool Motivated => _GUIChangeArrow > 0;

        private int _GUIChangeArrow;
        public override int GUIChangeArrow => _GUIChangeArrow;

        /// <summary>
        /// Indicates whenever pawn should be motivated.
        /// This property purpose is that pawn should be only motivated in semi-auto mode,
        /// which means after getting to full, it should wait to drop a bit before recharging again.
        /// </summary>
        public bool ShouldBeMotivated { get; set; }

        /// <summary>
        /// Indicates whenever pawn is currently working, and his motivation is decreasing by laziness rate.
        /// </summary>
        public bool IsPrisonerWorking { get; set; }

        /// <summary>
        /// Indicates whenever pawn is lazy and stopped working by lack of motivation.
        /// </summary>
        public bool IsLazy { get; private set; }

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
                ShouldBeMotivated = false;
            else if (CurLevel <= NeedInspirationLevel)
                ShouldBeMotivated = true;

            // Set IsLazy
            if (CurLevel <= LazyLevel && _GUIChangeArrow <= 0)
                IsLazy = true;
            else
                IsLazy = false;
        }

        private float GetChangePoints()
        {
            if (pawn.IsPrisoner && pawn.IsPrisonerOfColony)
            {
                if (pawn.GetRoomGroup() != null)
                {
                    var value = InspirationTracker.GetInsiprationValue(pawn, true);

                    if (PrisonLaborUtility.LaborEnabled(pawn))
                    {
                        if (IsPrisonerWorking)
                        {
                            value -= BGP.Laziness_LazyRate;
                            if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
                                value -= BGP.Laziness_HealthRate;
                            value -= (int)pawn.needs.food.CurCategory * BGP.Laziness_HungryRate;
                            // Some pawns have no rest need (e.g. Pawns with Circadian Half Cycler or androids from other mods)
                            if (pawn.needs.rest != null)
                            {
                                value -= (int) pawn.needs.rest.CurCategory * BGP.Laziness_TiredRate;
                            }
                        }
                        else if (pawn.timetable != null && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                        {
                            value += BGP.Laziness_JoyRate;
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
        }
    }
}