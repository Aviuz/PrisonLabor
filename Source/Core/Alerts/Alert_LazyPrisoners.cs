using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrisonLabor.Core.Components;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;

namespace PrisonLabor.Core.Alerts
{
    internal class Alert_LazyPrisoners : Alert
    {
        public Alert_LazyPrisoners()
        {
            defaultLabel = "PrisonLabor_LazyPrisonerAlert".Translate();
            defaultExplanation = "PrisonLabor_LazyPrisonerExplanationDef".Translate();
        }

        private IEnumerable<Pawn> LazyPrisoners
        {
            get
            {
                var maps = Find.Maps;
                foreach (int id in Tracked.index.Keys)
                {
                    PrisonerComp comp = (PrisonerComp)Tracked.pawnComps[id];
                    if (comp.IsPrisoner && !comp.motivated)
                        yield return (Pawn)comp.parent;
                }
            }
        }

        public override TaggedString GetExplanation()
        {
            Tutorials.Motivation();

            var stringBuilder = new StringBuilder();
            foreach (var current in LazyPrisoners)
                stringBuilder.AppendLine("    " + current.Name.ToStringShort);
            return string.Format("PrisonLabor_LazyPrisonerExplanation".Translate(), stringBuilder);
        }

        public override AlertReport GetReport()
        {
            if (PrisonLaborPrefs.EnableMotivationMechanics)
                return AlertReport.CulpritIs(LazyPrisoners.FirstOrDefault());
            return false;
        }
    }
}