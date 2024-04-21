using PrisonLabor.Constants;
using Multiplayer.API;
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
  public class ColumnWorker_Interaction : PawnColumnWorker
  {
    public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
    {
      if (pawn.guest != null)
      {
        Widgets.Dropdown(rect, pawn, (Pawn p) => p.guest.ExclusiveInteractionMode, Button_GenerateMenu, pawn.guest.ExclusiveInteractionMode.LabelCap.Truncate(rect.width), null, pawn.guest.ExclusiveInteractionMode.LabelCap, null, null, paintable: true);
      }
    }

    private IEnumerable<Widgets.DropdownMenuElement<PrisonerInteractionModeDef>> Button_GenerateMenu(Pawn pawn)
    {
      foreach (PrisonerInteractionModeDef intertaction in DefDatabase<PrisonerInteractionModeDef>.AllDefs
          .Where(def => pawn.CanUsePrisonerInteraction(def) && !def.isNonExclusiveInteraction)
          .OrderBy((PrisonerInteractionModeDef pim) => pim.listOrder))
      {
        yield return new Widgets.DropdownMenuElement<PrisonerInteractionModeDef>
        {
          option = new FloatMenuOption(intertaction.LabelCap, delegate
          {
            SetInteractionMode(pawn, intertaction);
          }),
          payload = intertaction
        };
      }
    }
    public void SetInteractionMode(Pawn pawn, PrisonerInteractionModeDef intertaction)
    {
      pawn.guest.SetExclusiveInteraction(intertaction);
      if (intertaction == PrisonerInteractionModeDefOf.Enslave && pawn.MapHeld != null && !PrisonLaborUtility.ColonyHasAnyWardenCapableOfEnslavement(pawn.MapHeld))
      {
        Messages.Message("MessageNoWardenCapableOfEnslavement".Translate(), pawn, MessageTypeDefOf.CautionInput, historical: false);
      }
      if (intertaction == PrisonerInteractionModeDefOf.Convert && pawn.guest.ideoForConversion == null)
      {
        pawn.guest.ideoForConversion = Faction.OfPlayer.ideos.PrimaryIdeo;
      }
      if (intertaction == PrisonerInteractionModeDefOf.Execution && pawn.MapHeld != null && !PrisonLaborUtility.ColonyHasAnyWardenCapableOfViolence(pawn.MapHeld))
      {
        Messages.Message("MessageCantDoExecutionBecauseNoWardenCapableOfViolence".Translate(), pawn, MessageTypeDefOf.CautionInput, historical: false);
      }
    }

    public override int GetMinWidth(PawnTable table)
    {
      return Mathf.Max(base.GetMinWidth(table), Mathf.CeilToInt(194f));
    }

    public override int GetOptimalWidth(PawnTable table)
    {
      return Mathf.Clamp(Mathf.CeilToInt(251f), GetMinWidth(table), GetMaxWidth(table));
    }

    public override int GetMinHeaderHeight(PawnTable table)
    {
      return Mathf.Max(base.GetMinHeaderHeight(table), 65);
    }

    public override int Compare(Pawn a, Pawn b)
    {
      return a.guest.ExclusiveInteractionMode.listOrder.CompareTo(b.guest.ExclusiveInteractionMode.listOrder);
    }
  }
}
