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
  public class ColumnWorker_IsWorking : PawnColumnWorker_Checkbox
  {
    protected override bool GetValue(Pawn pawn)
    {
      if (pawn == null)
      {
        DebugLogger.debug("Null pawn in ColumnWorker_IsWorking:GetValue");
        return true;
      }
      
      return pawn.guest.IsInteractionEnabled(PL_DefOf.PrisonLabor_workOption);
    }

    protected override void SetValue(Pawn pawn, bool value, PawnTable table)
    {
      if (pawn == null)
      {
        DebugLogger.debug("Null pawn in ColumnWorker_IsWorking:SetValue");
        return;
      }
      pawn.guest.ToggleNonExclusiveInteraction(PL_DefOf.PrisonLabor_workOption, value);
      table?.SetDirty();
    }
    protected override bool HasCheckbox(Pawn pawn)
    {
      return pawn.IsPrisonerOfColony;
    }
  }
}

