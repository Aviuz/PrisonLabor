using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Core.GUI_Components
{
    public class PawnTable_Prisoners : PawnTable
    {
        public PawnTable_Prisoners(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight)
        {
        }

        protected override IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
        {
            return PlayerPawnsDisplayOrderUtility.InOrder(input);
        }
    }
}
