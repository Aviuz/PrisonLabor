﻿using PrisonLabor.Constants;
using PrisonLabor.Core.Other;
using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class ColumnWorker_HasLegcuffs : PawnColumnWorker_Checkbox
    {
        protected override bool GetValue(Pawn pawn)
        {
            if (pawn == null)
            {
                DebugLogger.debug("Null pawn in ColumnWorker_HasLegcuffs:GetValue");
                return true;
            }
            CuffsTracker cuffsTracker = GetCuffsTracker(pawn);
            if (cuffsTracker != null && cuffsTracker.legscuffTracker.TryGetValue(pawn, out bool checkboxValue))
            {
                return checkboxValue;
            }
            return pawn.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedLegscuffs, false) == null;
        }

        protected override void SetValue(Pawn pawn, bool value, PawnTable table)
        {
            if (pawn == null)
            {
                DebugLogger.debug("Null pawn in ColumnWorker_HasLegcuffs:SetValue");
                return;
            }

            CuffsTracker cuffsTracker = GetCuffsTracker(pawn);
            if (cuffsTracker != null)
            {
                UpdateTracker(value, pawn, cuffsTracker);
            }
            table?.SetDirty();
        }

        private CuffsTracker GetCuffsTracker(Pawn pawn)
        {
            if (pawn.Map != null)
            {
                return pawn.Map.GetComponent<CuffsTracker>();
            }

            return null;
        }

        protected override bool HasCheckbox(Pawn pawn)
        {
            return pawn != null && pawn.health != null;
        }

        public void UpdateTracker(bool value, Pawn pawn, CuffsTracker cuffsTracker)
        {
            if (value == (pawn.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedLegscuffs, false) == null))
            {
                cuffsTracker.legscuffTracker.Remove(pawn);
            }
            else
            {
                cuffsTracker.legscuffTracker.Add(pawn, value);
            }
        }
    }
}
