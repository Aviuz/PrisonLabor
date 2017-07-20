using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    public class Need_Motivation : Need
    {
        private const float LazyLevel = 0.2f;
        private const float NeedInspirationLevel = 0.5f;
        private const float LazyRate = 0.003f;
        private const float HungryRate = 0.006f;
        private const float TiredRate = 0.006f;
        private const float HealthRate = 0.006f;
        private const float JoyRate = 0.006f;
        private const float InspireRate = 0.015f;
        public const int WardenCapacity = (int)(InspireRate / LazyRate);

        private static PrisonerInteractionModeDef pimDef;
        private static NeedDef def;

        private bool enabled;
        private bool needToBeInspired;
        private bool isLazy;
        private int wardensCount;
        private int prisonersCount;

        private int slowDown;
        private int guiArrow;

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        public bool NeedToBeInspired
        {
            get
            {
                return needToBeInspired;
            }
        }

        public bool IsLazy
        {
            get
            {
                return isLazy;
            }
        }

        public float PercentageThreshNeedInsipration
        {
            get
            {
                return NeedInspirationLevel;
            }
        }

        public float PercentageThreshLazy
        {
            get
            {
                return LazyLevel;
            }
        }

        //TODO change to lazy category?
        public HungerCategory CurCategory
        {
            get
            {
                return 0;
            }
        }

        public override int GUIChangeArrow
        {
            get
            {
                return guiArrow;
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

        public static PrisonerInteractionModeDef PimDef
        {
            get
            {
                if (pimDef == null)
                    pimDef = DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption");
                return pimDef;
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
                        List<Pawn> pawnsInRoom = new List<Pawn>();
                        prisonersCount = 0;
                        wardensCount = 0;
                        foreach (IntVec3 cell in pawn.GetRoomGroup().Cells)
                        {
                            foreach (Thing thing in cell.GetThingList(pawn.Map))
                            {
                                if (thing is Pawn)
                                    pawnsInRoom.Add((Pawn)thing);
                            }
                        }
                        foreach (Pawn p in pawnsInRoom)
                        {
                            // colonist nearby
                            if (p.IsFreeColonist)
                                wardensCount++;
                            if (p.IsPrisoner && p.guest.interactionMode == PimDef)
                                prisonersCount++;
                        }

                        if (pawn.guest.interactionMode == PimDef)
                        {
                            float value = wardensCount * InspireRate / prisonersCount;
                            if (enabled)
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
                            guiArrow = value.CompareTo(0.0f);
                            return value;
                        }
                        else
                        {
                            float value = wardensCount * InspireRate / (prisonersCount + 1);
                            guiArrow = value.CompareTo(0.0f);
                            return value;
                        }
                    }
                    else
                    {
                        guiArrow = 0;
                        return 0.0f;
                    }
                }
                else
                {
                    guiArrow = 1;
                    return +0.01f;
                }
            }
        }

        public Need_Motivation(Pawn pawn) : base(pawn)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Values.Look<int>(ref this.lastNonStarvingTick, "lastNonStarvingTick", -99999, false);
        }

        public override void NeedInterval()
        {
            //for perfomance purposes
            if (slowDown < 5)
            {
                CurLevel += LazinessRate;

                if (CurLevel == MaxLevel)
                    needToBeInspired = false;
                if (CurLevel <= NeedInspirationLevel && !needToBeInspired)
                    needToBeInspired = true;
                if (CurLevel <= LazyLevel && !isLazy && wardensCount == 0)
                {
                    isLazy = true;
                    Messages.Message("PrisonLabor_LazyPrisonerMessage".Translate(), pawn, MessageSound.Standard);
                    Tutorials.Motivation();
                }
                else if (isLazy && wardensCount > 0)
                {
                    isLazy = false;
                }

                slowDown = 0;
            }
            else
            {
                slowDown++;
            }
        }

        public override void SetInitialLevel()
        {
            CurLevelPercentage = 1.0f;
            enabled = false;
        }
       
        public override string GetTipString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetTipString());
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("WardenResponseThreshold".Translate() + ": " + PercentageThreshNeedInsipration.ToStringPercent());
            stringBuilder.AppendLine("StoppingWorkThreshold".Translate() + ": " + PercentageThreshLazy.ToStringPercent());
            return stringBuilder.ToString();
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true)
        {
            if (this.threshPercents == null)
            {
                this.threshPercents = new List<float>();
            }
            this.threshPercents.Clear();
            this.threshPercents.Add(this.PercentageThreshLazy);
            this.threshPercents.Add(this.PercentageThreshNeedInsipration);
            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
        }
    }
}
