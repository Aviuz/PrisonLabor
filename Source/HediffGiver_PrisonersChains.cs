using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace PrisonLabor
{
    class HediffGiver_PrisonersChains : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.hediff, false);

            float value;
            if (pawn.IsPrisoner)
                value = 1.0f;
            else
                value = 0.0f;

            if (hediff != null)
            {
                hediff.Severity = value;
            }
            else if (value != 0)
            {
                hediff = HediffMaker.MakeHediff(this.hediff, pawn, null);
                hediff.Severity = value;
                pawn.health.AddHediff(hediff, null, null);
            }
        }

        public static void Init()
        {
            var giver = new HediffGiver_PrisonersChains();
            giver.hediff = DefDatabase<HediffDef>.GetNamed("PrisonLabor_PrisonerChains");
            DefDatabase<HediffGiverSetDef>.GetNamed("OrganicStandard").hediffGivers.Add(giver);
        }
    }
}
