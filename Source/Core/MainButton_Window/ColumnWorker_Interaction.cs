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
                List<PrisonerInteractionModeDef> lists = new List<PrisonerInteractionModeDef>();
                Widgets.Dropdown(rect, pawn, (Pawn p) => p.guest.interactionMode, Button_GenerateMenu, pawn.guest.interactionMode.LabelCap.Truncate(rect.width), null, pawn.guest.interactionMode.LabelCap, null, null, paintable: true);
                
            }
        }

        private IEnumerable<Widgets.DropdownMenuElement<PrisonerInteractionModeDef>> Button_GenerateMenu(Pawn pawn)
        {
            foreach (PrisonerInteractionModeDef intertaction in DefDatabase<PrisonerInteractionModeDef>.AllDefs.OrderBy((PrisonerInteractionModeDef pim) => pim.listOrder))
            {
                yield return new Widgets.DropdownMenuElement<PrisonerInteractionModeDef>
                {
                    option = new FloatMenuOption(intertaction.LabelCap, delegate
                    {
                        pawn.guest.interactionMode = intertaction;
                    }),
                    payload = intertaction
                };
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

            return a.guest.interactionMode.listOrder.CompareTo(b.guest.interactionMode.listOrder);
        }
    }
}
