using PrisonLabor.Constants;
using PrisonLabor.Core.Components;
using PrisonLabor.Core.Other;
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
  public class ColumnWorker_HasIntel : PawnColumnWorker_Checkbox
  {
    protected override bool GetValue(Pawn pawn)
    {
      if (pawn == null)
      {
        DebugLogger.debug("Null pawn in ColumnWorker_HasIntel:GetValue");
        return true;
      }
      PrisonerComp prisonerComp = pawn.GetComp<PrisonerComp>();
      return prisonerComp != null && prisonerComp.HasIntel;
    }

    protected override void SetValue(Pawn pawn, bool value, PawnTable table)
    {
      if (pawn == null)
      {
        DebugLogger.debug("Null pawn in ColumnWorker_HasIntel:SetValue");
        return;
      }
      table?.SetDirty();
    }
    protected override bool HasCheckbox(Pawn pawn)
    {
      return pawn != null && pawn.GetComp<PrisonerComp>() != null;
    }
  }
}

