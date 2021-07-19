using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class ColumnWorker_EscapeTracker : PawnColumnWorker_Text
    {
        protected override string GetTextFor(Pawn pawn)
        {
            
            var escapeTracker = EscapeTracker.Of(pawn);
            if(escapeTracker != null)
            {
                return (escapeTracker.ReadyToEscape ? "ready" : escapeTracker.ReadyToRunPercentage + " %");
            }

            return "0 %";
        }

        protected override string GetTip(Pawn pawn)
        {
            var escapeTracker = EscapeTracker.Of(pawn);
            if (escapeTracker != null)
            {
                return $"(Cap:{ escapeTracker.EscapeLevel})";
            }
            return "";
        }
    }
}
