using PrisonLabor.Core.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Core.AI.ThoughtWorkers
{
    public class ThoughtWorker_TreatedBadly : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.IsPrisoner)
                return false;

            var treatmant = p.needs.TryGetNeed<Need_Treatment>();

            if (treatmant == null)
                return false;

            return treatmant.CurLevelPercentage < 0.25;
        }
    }
}
