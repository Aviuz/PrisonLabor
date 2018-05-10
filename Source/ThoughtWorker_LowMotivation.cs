using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    public class ThoughtWorker_LowMotivation : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.IsPrisoner)
                return false;
            var need = p.needs.TryGetNeed<Need_Motivation>();
            return need != null && need.IsLazy;
        }
    }
}
