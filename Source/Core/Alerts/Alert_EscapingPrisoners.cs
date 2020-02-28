using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Other;
using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Alerts
{
    public class Alert_EscapingPrisoners : Alert_Critical
    {
        public Alert_EscapingPrisoners()
        {
            defaultLabel = "PrisonLabor_Alert_EscapingPrisoners_Title".Translate();
            defaultExplanation = "PrisonLabor_Alert_EscapingPrisoners_DefaultExplanation".Translate();
        }

        private IEnumerable<Pawn> PotentialEscapingPrisoners
        {
            get
            {
                var maps = Find.Maps;
                for (var i = 0; i < maps.Count; i++)
                    foreach (var pawn in maps[i].mapPawns.AllPawns.Where(p=>p.IsPrisoner && EscapeTracker.Of(p, true).CanEscape))
                            yield return pawn;
            }
        }

        public override TaggedString GetExplanation()
        {
            Tutorials.Motivation();

            var stringBuilder = new StringBuilder();
            foreach (var current in PotentialEscapingPrisoners)
                stringBuilder.AppendLine("    " + current.Name.ToStringShort);
            return new TaggedString(string.Format("PrisonLabor_Alert_EscapingPrisoners_ExplanationFormat".Translate(), stringBuilder.ToString()));
        }

        public override AlertReport GetReport()
        {
            if (PrisonLaborPrefs.EnableMotivationMechanics)
                return AlertReport.CulpritIs(PotentialEscapingPrisoners.FirstOrDefault());
            return false;
        }
    }
}
