using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Core.Alerts
{
    internal class Alert_StarvingPrisoners : Alert
    {
        public Alert_StarvingPrisoners()
        {
            defaultLabel = "PrisonLabor_StarvingPrisonerAlert".Translate();
            defaultExplanation = "PrisonLabor_StarvingPrisonerExplanationDef".Translate();
        }

        private IEnumerable<Pawn> StarvingPrisoners
        {
            get
            {
                var maps = Find.Maps;
                for (var i = 0; i < maps.Count; i++)
                    foreach (var pawn in maps[i].mapPawns.AllPawns)
                        if (PrisonLaborUtility.LaborEnabled(pawn) && PrisonLaborUtility.WorkTime(pawn) &&
                            (!PrisonLaborPrefs.EnableMotivationMechanics ||
                             !pawn.needs.TryGetNeed<Need_Motivation>().IsLazy) && pawn.timetable != null &&
                            pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything &&
                            pawn.needs.food.Starving)
                            yield return pawn;
            }
        }

        public override string GetExplanation()
        {
            var stringBuilder = new StringBuilder();
            foreach (var current in StarvingPrisoners)
                stringBuilder.AppendLine("    " + current.Name.ToStringShort);
            return string.Format("PrisonLabor_StarvingPrisonerExplanation".Translate(), stringBuilder);
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritIs(StarvingPrisoners.FirstOrDefault());
        }
    }
}