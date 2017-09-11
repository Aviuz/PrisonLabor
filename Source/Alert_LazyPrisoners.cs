using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PrisonLabor
{
    internal class Alert_LazyPrisoners : Alert
    {
        public Alert_LazyPrisoners()
        {
            defaultLabel = "Prisoners aren't working";
            defaultExplanation = "Work in progress";
        }

        private IEnumerable<Pawn> LazyPrisoners
        {
            get
            {
                var maps = Find.Maps;
                for (var i = 0; i < maps.Count; i++)
                    foreach (var pawn in maps[i].mapPawns.AllPawns)
                        if (PrisonLaborUtility.LaborEnabled(pawn) && PrisonLaborUtility.WorkTime(pawn) &&
                            pawn.needs.TryGetNeed<Need_Motivation>().IsLazy)
                            yield return pawn;
            }
        }

        public override string GetExplanation()
        {
            var stringBuilder = new StringBuilder();
            foreach (var current in LazyPrisoners)
                stringBuilder.AppendLine("    " + current.NameStringShort);
            return string.Format("Those prisoners are lazy:\n\n{0}\nTry to motivate them.", stringBuilder);
        }

        public override AlertReport GetReport()
        {
            if (PrisonLaborPrefs.EnableMotivationMechanics)
                return AlertReport.CulpritIs(LazyPrisoners.FirstOrDefault());
            return false;
        }
    }
}