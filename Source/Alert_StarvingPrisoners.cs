using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    class Alert_StarvingPrisoners : Alert
    {
        public Alert_StarvingPrisoners()
        {
            this.defaultLabel = "Prisoners are starving";
            this.defaultExplanation = "Work in progress";
        }

        private IEnumerable<Pawn> StarvingPrisoners
        {
            get
            {
                List<Map> maps = Find.Maps;
                for (int i = 0; i < maps.Count; i++)
                {
                    foreach (Pawn pawn in maps[i].mapPawns.AllPawns)
                    {
                        if (PrisonLaborUtility.LaborEnabled(pawn) && PrisonLaborUtility.WorkTime(pawn) && !pawn.needs.TryGetNeed<Need_Motivation>().IsLazy && pawn.timetable != null && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything && pawn.needs.food.Starving)
                            yield return pawn;
                    }
                }
            }
        }

        public override string GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn current in StarvingPrisoners)
            {
                stringBuilder.AppendLine("    " + current.NameStringShort);
            }
            return string.Format("Those prisoners are starving and won't work:\n\n{0}", stringBuilder.ToString());
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritIs(StarvingPrisoners.FirstOrDefault());
        }
    }
}
