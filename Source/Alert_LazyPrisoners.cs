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
            this.defaultLabel = "Prisoners aren't working";
            this.defaultExplanation = "Work in progress";
        }

        private IEnumerable<Pawn> LazyPrisoners
        {
            get
            {
                List<Map> maps = Find.Maps;
                for (int i = 0; i < maps.Count; i++)
                {
                    foreach (Pawn pawn in maps[i].mapPawns.AllPawns)
                    {
                        if (PrisonLaborUtility.LaborEnabled(pawn) && PrisonLaborUtility.WorkTime(pawn) && pawn.needs.TryGetNeed<Need_Motivation>().IsLazy)
                            yield return pawn;
                    }
                }
            }
        }

        public override string GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn current in LazyPrisoners)
            {
                stringBuilder.AppendLine("    " + current.NameStringShort);
            }
            return string.Format("Those prisoners are lazy:\n\n{0}\nTry to motivate them.", stringBuilder.ToString());
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritIs(LazyPrisoners.FirstOrDefault());
        }

    }
}
