using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    public class ThoughtWorker_FreeTime : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.IsPrisoner)
                return false;
            return p.timetable != null && p.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy;
        }
    }
}
