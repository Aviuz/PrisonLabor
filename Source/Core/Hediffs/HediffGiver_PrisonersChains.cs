using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using PrisonLabor.Constants;

namespace PrisonLabor.Core.Hediffs
{
    class HediffGiver_PrisonersChains : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.hediff, false);
            float mod = 0f;
            if (pawn != null && pawn.IsPrisonerOfColony)
            {
                if (pawn.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedHandscuffs, false) != null)
                {
                    mod += 0.19f;
                }
                if (pawn.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedLegscuffs, false) != null)
                {
                    mod += 0.19f * 2f;
                    
                }
            }
            float value;
            if (RestraintsUtility.InRestraints(pawn))
                value = 1.0f - mod;
            else if (hediff != null && pawn.Faction != Faction.OfPlayer)
                value = hediff.Severity - 0.01f;
            else
                value = 0.0f;


            if (pawn.guest != null && pawn.guest.Released)
            {
                value = 0f;
            }

            if (hediff != null)
            {
                hediff.Severity = value;
            }
            else if (value != 0 && pawn.IsPrisonerOfColony)
            {
                hediff = HediffMaker.MakeHediff(this.hediff, pawn, null);
                hediff.Severity = value;
                pawn.health.AddHediff(hediff, null, null);
            }
        }
    }
}
