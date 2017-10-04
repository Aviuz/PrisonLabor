using System.Collections.Generic;
using System.Text;
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
        private const float LazyRate = 0.002f;
        private const float HungryRate = 0.006f;
        private const float TiredRate = 0.006f;
        private const float HealthRate = 0.006f;
        private const float JoyRate = 0.006f;
        private const int ReadyToRunLevel = 100;

        private static NeedDef def;

        private int delta;
        private int impatient;

        public Need_Motivation(Pawn pawn) : base(pawn)
        {
            delta = 0;
            impatient = 0;
            ReadyToRun = false;
            Insipred = false;
        }

        public bool Enabled { get; set; }

        public bool NeedToBeInspired { get; private set; }

        public bool IsLazy { get; private set; }

        public bool ReadyToRun { get; private set; }

        public bool Insipred { get; private set; }

        public bool CanEscape { get; set; }

        public float PercentageThreshNeedInsipration => NeedInspirationLevel;

        public float PercentageThreshLazy => LazyLevel;

        //TODO change to lazy category?
        public HungerCategory CurCategory => 0;

        public override int GUIChangeArrow => delta;

        public bool Motivated
        {
            get
            {
                if (delta == 1)
                    return true;
                return false;
            }
        }

        public static NeedDef Def
        {
            get
            {
                if (def == null)
                    def = DefDatabase<NeedDef>.GetNamed("PrisonLabor_Motivation");
                return def;
            }
        }

        private float LazinessRate
        {
            get
            {
                if (pawn.IsPrisoner && pawn.IsPrisonerOfColony)
                {
                    if (pawn.GetRoomGroup() != null)
                    {
                        var value = InspirationUtility.GetInsiprationValue(pawn);

                        if (PrisonLaborUtility.LaborEnabled(pawn))
                        {
                            if (Enabled)
                            {
                                value -= LazyRate;
                                if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
                                    value -= HealthRate;
                                value -= (int)pawn.needs.food.CurCategory * HungryRate;
                                value -= (int)pawn.needs.rest.CurCategory * TiredRate;
                                if (value >= 0)
                                    Insipred = true;
                                else
                                    Insipred = false;
                            }
                            else if (pawn.timetable != null &&
                                     pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                            {
                                if (value != 0)
                                    Insipred = true;
                                else
                                    Insipred = false;
                                value += JoyRate;
                            }
                            else
                            {
                                if (value != 0)
                                    Insipred = true;
                                else
                                    Insipred = false;
                            }
                            delta = value.CompareTo(0.0f);
                            return value;
                        }
                        else
                        {
                            if (value != 0)
                                Insipred = true;
                            else
                                Insipred = false;

                            delta = value.CompareTo(0.0f);
                            return value;
                        }
                    }
                    else
                    {
                        delta = 0;
                        return 0.0f;
                    }
                }
                delta = 1;
                return +0.01f;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Values.Look<int>(ref this.lastNonStarvingTick, "lastNonStarvingTick", -99999, false);
        }

        public override void NeedInterval()
        {
            CurLevel += LazinessRate;

            if (CurLevel == MaxLevel)
                NeedToBeInspired = false;
            if (CurLevel <= NeedInspirationLevel && !NeedToBeInspired)
                NeedToBeInspired = true;
            if (CurLevel <= LazyLevel && !IsLazy && delta <= 0)
            {
                IsLazy = true;
                Tutorials.Motivation();
            }
            else if (IsLazy && delta > 0)
            {
                IsLazy = false;
            }

            ImpatientTick();
        }

        public override void SetInitialLevel()
        {
            CurLevelPercentage = 1.0f;
            Enabled = false;
        }

        public override string GetTipString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetTipString());
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("PrisonLabor_WardenResponseThreshold".Translate() + ": " +
                                     PercentageThreshNeedInsipration.ToStringPercent());
            stringBuilder.AppendLine(
                "PrisonLabor_StoppingWorkThreshold".Translate() + ": " + PercentageThreshLazy.ToStringPercent());
            return stringBuilder.ToString();
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f,
            bool drawArrows = true, bool doTooltip = true)
        {
            if (threshPercents == null)
                threshPercents = new List<float>();
            threshPercents.Clear();
            threshPercents.Add(PercentageThreshLazy);
            threshPercents.Add(PercentageThreshNeedInsipration);
            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
        }

        private void ImpatientTick()
        {
            if (Insipred || !CanEscape)
            {
                if (impatient != 0)
                {
                    impatient = 0;
                    ReadyToRun = false;
                }
            }
            else if (!ReadyToRun)
            {
                impatient++;
                if (impatient >= ReadyToRunLevel)
                {
                    ReadyToRun = true;
                }
            }
        }
    }
}