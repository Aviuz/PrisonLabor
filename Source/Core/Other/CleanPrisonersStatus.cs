using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Other
{
    public static class CleanPrisonersStatus
    {

        static public void Clean(Pawn prisoner)
        {
            prisoner.workSettings = new Pawn_WorkSettings(prisoner);
            Hediff legs = prisoner.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedLegscuffs, false);
            if (legs != null)
            {
                prisoner.health.hediffSet.hediffs.Remove(legs);
            }
            Hediff hands = prisoner.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedHandscuffs, false);
            if (hands != null)
            {
                prisoner.health.hediffSet.hediffs.Remove(hands);
            }
            prisoner.playerSettings.AreaRestriction = null;
        }
    }
}
