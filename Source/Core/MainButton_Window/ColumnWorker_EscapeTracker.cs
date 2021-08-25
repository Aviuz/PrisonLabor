using PrisonLabor.Core.Components;
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
            var prisonerComp = pawn.TryGetComp<PrisonerComp>();
            if(prisonerComp != null)
            {
                return (prisonerComp.escapeTracker.ReadyToEscape ? "ready" : prisonerComp.escapeTracker.ReadyToRunPercentage + " %");
            }

            return null;
        }

        protected override string GetTip(Pawn pawn)
        {
            var prisonerComp = pawn.TryGetComp<PrisonerComp>();
            if (prisonerComp != null)
            {
                return $"(Cap:{ prisonerComp.escapeTracker.EscapeLevel})";
            }
            return null;
        }
    }
}
