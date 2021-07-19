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
    public class ColumnWorker_Motivation : PawnColumnWorker_Text
    {
        protected override string GetTextFor(Pawn pawn)
        {
            Need_Motivation motivation = pawn.needs.TryGetNeed<Need_Motivation>();
            if (motivation != null)
            {
                int value = (int)(motivation.CurLevelPercentage * 100f);
                return $"{value} %";
            }
            return "";
        }
    }
}
