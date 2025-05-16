using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
  public abstract class CustomTabWindow : MainTabWindow_PawnTable
  {
    protected override IEnumerable<Pawn> Pawns => Find.CurrentMap.mapPawns.PrisonersOfColony;

    protected CustomTabWindow()
    {
      def = UIDefsOf.PL_Prisoners_Menu;
    }
  }
}
