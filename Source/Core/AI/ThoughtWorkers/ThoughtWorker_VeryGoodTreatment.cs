using PrisonLabor.Core.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Core.AI.ThoughtWorkers
{
    public class ThoughtWorker_VeryGoodTreatment : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.IsPrisoner)
                return false;
            var need = p.needs.TryGetNeed<Need_Treatment>();
            return need != null && need.CurCategory == TreatmentCategory.VeryGood;
        }
    }
}
