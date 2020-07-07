using PrisonLabor.Core.Components;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Trackers;
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
                foreach (int id in Tracked.index.Keys)
                {
                    PrisonerComp comp = (PrisonerComp)Tracked.pawnComps[id];
                    if (comp.IsPrisoner && comp.IsStarving)
                        yield return (Pawn)comp.parent;
                }
            }
        }

        public override TaggedString GetExplanation()
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