using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    class Alert_LazyPrisoners : Alert
    {
        public Alert_LazyPrisoners()
        {
            this.defaultLabel = "Your prisoners are not working";
            this.defaultExplanation = "Work in progress";
        }

        public override AlertReport GetReport()
        {
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                if (AnyLazyPrisoners(maps[i].mapPawns.AllPawns))
                {
                    return true;
                }
            }
            return false;
        }

        private bool AnyLazyPrisoners(IEnumerable<Pawn> pawns)
        {
            foreach(Pawn pawn in pawns)
            {
                if (pawn.IsPrisoner && pawn.guest.interactionMode == PrisonLaborUtility.PIM_Def && pawn.needs.TryGetNeed<Need_Motivation>().IsLazy)
                    return true;
            }
            return false;
        }

    }
}
