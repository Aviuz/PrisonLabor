using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    public class Need_Laziness : Need
    {
        private const float LazyLevel = 0.8f;
        private const float NeedInspirationLevel = 0.5f;
        private const float LazyRate = 0.003f;
        private const float InspireRate = 0.015f;
        public const int WardenCapacity = (int)(InspireRate / LazyRate);

        private static PrisonerInteractionModeDef pimDef;
        private static NeedDef def;

        private bool needToBeInspired;
        private bool isLazy;
        private int wardensCount;
        private int prisonersCount;

        private int slowDown;

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
                if (wardensCount * WardenCapacity < prisonersCount)
                    return 1;
                else if (wardensCount * WardenCapacity > prisonersCount)
                    return -1;
                else
                    return 0;
            }
        }

        public static NeedDef Def
        {
            get
            {
                if (def == null)
                    def = DefDatabase<NeedDef>.GetNamed("PrisonLabor_Laziness");
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
                            return LazyRate - wardensCount * InspireRate / prisonersCount;
                        else
                            return -wardensCount * InspireRate / (prisonersCount + 1);
                    }
                    else
                    {
                        return 0.0f;
                    }
                }
                else
                {
                    return -0.01f;
                }
            }
        }

        public Need_Laziness(Pawn pawn) : base(pawn)
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

                if (CurLevel == 0)
                    needToBeInspired = false;
                if (CurLevel >= NeedInspirationLevel && !needToBeInspired)
                    needToBeInspired = true;
                if (CurLevel >= LazyLevel && !isLazy && wardensCount == 0)
                {
                    isLazy = true;
                    Messages.Message("PrisonLabor_LazyPrisonerMessage".Translate(), pawn, MessageSound.Standard);
                    Tutorials.LazyPrisoner();
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
            CurLevel = 0.0f;
        }

        public override string GetTipString()
        {
            return string.Concat(new string[]
            {
                base.LabelCap,
                ": ",
                base.CurLevelPercentage.ToStringPercent(),
                " (",
                CurLevel.ToString("0.##"),
                " / ",
                MaxLevel.ToString("0.##"),
                ")\n",
                Def.description
            });
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
