using Multiplayer.API;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
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
    public class ColumnWorker_Resocialization : PawnColumnWorker
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            var need = pawn.needs.TryGetNeed<Need_Treatment>();
            if (need != null && need.ResocializationReady)
            {
                if(Widgets.ButtonText(rect, "PrisonLabor_RecruitButtonLabel".Translate()))
                {
                    ConvertPrisoner(pawn);
                }

            }
        }
        public void ConvertPrisoner(Pawn pawn)
        {
            CleanPrisonersStatus.Clean(pawn);
            pawn.guest.SetGuestStatus(null);
            pawn.SetFaction(Faction.OfPlayer);
        }
    }
}
