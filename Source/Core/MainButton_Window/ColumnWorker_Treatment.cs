using PrisonLabor.Core.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class ColumnWorker_Treatment : PawnColumnWorker_Text
    {
        protected override string GetTextFor(Pawn pawn)
        {
            Need_Treatment treatmeant = pawn.needs.TryGetNeed<Need_Treatment>();
            if (treatmeant != null)
            {
                int value = (int)(treatmeant.CurLevelPercentage * 100f);
                return $"{value} %";
            }
            return "";
        }
    }
}
