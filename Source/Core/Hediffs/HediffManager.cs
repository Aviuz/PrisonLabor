using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Core.Hediffs
{
    public static class HediffManager
    {
        public static void Init()
        {
            var giver = new HediffGiver_PrisonersChains();
            giver.hediff = DefDatabase<HediffDef>.GetNamed("PrisonLabor_PrisonerChains");
            DefDatabase<HediffGiverSetDef>.GetNamed("OrganicStandard").hediffGivers.Add(giver);
        }
    }
}
